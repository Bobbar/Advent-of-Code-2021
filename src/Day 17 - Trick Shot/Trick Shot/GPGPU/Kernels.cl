
__kernel void FindMaxY(int len, int2 targTopLeft, int2 targBotRight, int startVelo, global int* maxY)
{
	int idX = get_global_id(0);
	int idY = get_global_id(1);

	int2 velo = (int2)(startVelo + idX, startVelo + idY);
	int2 pos = (int2)(0, 0);

	int maxYPos = 0;
	bool done = false;
	bool wasHit = false;
	while (!done)
	{
		pos += velo;

		if (velo.x > 0)
			velo.x--;
		else if (velo.x < 0)
			velo.x++;

		velo.y--;

		maxYPos = max(maxYPos, pos.y);

		if (pos.x >= targTopLeft.x && pos.x <= targBotRight.x && pos.y >= targBotRight.y && pos.y <= targTopLeft.y)
		{
			wasHit = true;
			break;
		}

		if (pos.x > targBotRight.x || pos.y < targBotRight.y)
			break;
	}

	if (wasHit)
		maxY[get_global_linear_id()] = maxYPos;
}


__kernel void FindHits(int len, int2 targTopLeft, int2 targBotRight, int startVelo, global int2* hits)
{
	int idX = get_global_id(0);
	int idY = get_global_id(1);

	int2 initVelo = (int2)(startVelo + idX, startVelo + idY);
	int2 velo = initVelo;
	int2 pos = (int2)(0, 0);

	bool done = false;
	bool wasHit = false;
	while (!done)
	{
		pos += velo;

		if (velo.x > 0)
			velo.x--;
		else if (velo.x < 0)
			velo.x++;

		velo.y--;

		if (pos.x >= targTopLeft.x && pos.x <= targBotRight.x && pos.y >= targBotRight.y && pos.y <= targTopLeft.y)
		{
			wasHit = true;
			break;
		}

		if (pos.x > targBotRight.x || pos.y < targBotRight.y)
			break;
	}

	if (wasHit)
		hits[get_global_linear_id()] = initVelo;
}