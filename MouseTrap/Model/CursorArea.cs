using System;
using MouseTrap.Hooks;

namespace MouseTrap.Model
{
    public class CursorArea : IEquatable<CursorArea>
    {
        public int MinX { get; }
        public int MinY { get; }
        public int MaxX { get; }
        public int MaxY { get; }


        public CursorArea(WindowsAPI.RectStruct rect)
        {
            MinX = rect.Left;
            MinY = rect.Top;
            MaxX = rect.Right;
            MaxY = rect.Bottom;
        }


        public bool Equals(CursorArea other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return MinX == other.MinX && MinY == other.MinY && MaxX == other.MaxX && MaxY == other.MaxY;
        }

        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            return obj is CursorArea cursorArea && Equals(cursorArea);
        }
        

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MinX;
                hashCode = (hashCode * 397) ^ MinY;
                hashCode = (hashCode * 397) ^ MaxX;
                hashCode = (hashCode * 397) ^ MaxY;
                return hashCode;
            }
        }
    }
}
