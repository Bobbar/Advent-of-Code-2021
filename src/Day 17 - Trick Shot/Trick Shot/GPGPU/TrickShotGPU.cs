using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloo;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Trick_Shot.GPGPU
{
    public class TrickShotGPU : IDisposable
    {
        private ComputeContext _context;
        private ComputeCommandQueue _queue;
        private ComputeDevice _device = null;
        private ComputeProgram _program;

        public TrickShotGPU()
        {
            Init();
        }

        private void Init()
        {
            int gpuIdx = 0;
            var devices = new List<ComputeDevice>();

            foreach (var plat in ComputePlatform.Platforms)
                foreach (var dev in plat.Devices)
                    devices.Add(dev);

            _device = devices[gpuIdx];

            var platform = _device.Platform;
            _context = new ComputeContext(new[] { _device }, new ComputeContextPropertyList(platform), null, IntPtr.Zero);
            _queue = new ComputeCommandQueue(_context, _device, ComputeCommandQueueFlags.None);

            var streamReader = new StreamReader(Environment.CurrentDirectory + $@"\GPGPU\Kernels.cl");
            var clSource = streamReader.ReadToEnd();
            streamReader.Close();

            _program = new ComputeProgram(_context, clSource);

            try
            {
                _program.Build(new[] { _device }, "-cl-std=CL2.0", null, IntPtr.Zero);
            }
            catch (BuildProgramFailureComputeException ex)
            {
                string buildLog = _program.GetBuildLog(_device);
                Debug.WriteLine(buildLog);
                throw;
            }

        }

        public int FindMaxY(int minVelo, int maxVelo, Point targTopLeft, Point targBotRight)
        {
            int vDiff = maxVelo - minVelo;
            int len = vDiff * vDiff;
            int threadsPB = 1024;
            int blocks = BlockCount(vDiff, threadsPB);
            var maxYPos = new int[len];

            using (var maxYBuf = new ComputeBuffer<int>(_context, ComputeMemoryFlags.ReadWrite, len))
            using (var kern = _program.CreateKernel("FindMaxY"))
            {
                kern.SetValueArgument(0, len);
                kern.SetValueArgument(1, new int2(targTopLeft.X, targTopLeft.Y));
                kern.SetValueArgument(2, new int2(targBotRight.X, targBotRight.Y));
                kern.SetValueArgument(3, minVelo);
                kern.SetMemoryArgument(4, maxYBuf);
                _queue.Execute(kern, null, new long[] { blocks * vDiff, blocks * vDiff }, null, null);
                _queue.ReadFromBuffer(maxYBuf, ref maxYPos, true, null);
                _queue.Finish();
            }

            int max = maxYPos.Max();
            return max;
        }

        public List<Point> FindHits(int minVelo, int maxVelo, Point targTopLeft, Point targBotRight)
        {
            int vDiff = (maxVelo + 1) - (minVelo);
            int len = vDiff * vDiff;
            int threadsPB = 1024;
            int blocks = BlockCount(vDiff, threadsPB);
            var hits = new int2[len];

            using (var hitsBuff = new ComputeBuffer<int2>(_context, ComputeMemoryFlags.ReadWrite, len))
            using (var kern = _program.CreateKernel("FindHits"))
            {
                kern.SetValueArgument(0, len);
                kern.SetValueArgument(1, new int2(targTopLeft.X, targTopLeft.Y));
                kern.SetValueArgument(2, new int2(targBotRight.X, targBotRight.Y));
                kern.SetValueArgument(3, minVelo);
                kern.SetMemoryArgument(4, hitsBuff);
                _queue.Execute(kern, null, new long[] { blocks * vDiff, blocks * vDiff }, null, null);
                _queue.ReadFromBuffer(hitsBuff, ref hits, true, null);
                _queue.Finish();
            }

            var hitsOnly = hits.Where(h => h.X + h.Y > 0).ToList();
            var hitsPoints = new List<Point>();
            hitsOnly.ForEach(h => hitsPoints.Add(new Point(h.X, h.Y)));

            return hitsPoints;
        }

        private int BlockCount(int len, int threads)
        {
            var blocks = (int)Math.Round((len - 1 + threads - 1) / (float)threads, 0);

            if (((threads * blocks) - len) > threads)
            {
                blocks -= 1;
            }
            else if ((threads * blocks) < len)
            {
                blocks += 1;
            }

            return blocks;
        }

        public void Dispose()
        {
            _context?.Dispose();
            _queue?.Dispose();
            _program?.Dispose();
        }
    }
}
