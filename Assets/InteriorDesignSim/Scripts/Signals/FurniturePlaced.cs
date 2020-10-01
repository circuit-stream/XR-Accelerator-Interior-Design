using Signals;
using XRAccelerator.Configs;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Signals
{
    public class FurniturePlaced : Signal
    {
        public FurnitureConfig Config;
        public FurnitureGraphics Graphics;
    }
}