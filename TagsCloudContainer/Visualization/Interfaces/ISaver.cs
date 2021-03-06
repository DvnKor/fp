﻿using System.Drawing;
using TagsCloudContainer.ResultInfrastructure;

namespace TagsCloudContainer.Visualization.Interfaces
{
    public interface ISaver
    {
        Result SaveImage(string path, Bitmap bitmap, Size resolution);
    }
}