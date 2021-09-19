using System;
using OpenTK.Mathematics;
using Math = System.MathF;

namespace Cubic.Utilities
{
    public static class Physics
    {

        public static bool Raycast(Func<Vector3, bool> isVoxel, Vector3 position, Vector3 direction, uint maxDistance, out RaycastHit hit)
        {
            //float ds = MathF.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
            //direction.X /= ds;
            //direction.Y /= ds;
            //direction.Z /= ds;

            const float stepAmount = 0.1f;
            
            float t = 0;
            Vector3 i = new Vector3(position.X, position.Y, position.Z);
            Vector3 step = new Vector3(direction.X > 0 ? stepAmount : -stepAmount,
                direction.Y > 0 ? stepAmount : -stepAmount, direction.Z > 0 ? stepAmount : -stepAmount);
            Vector3 tDelta = new Vector3(Math.Abs(1 / direction.X), Math.Abs(1 / direction.Y),
                Math.Abs(1 / direction.Z));
            Vector3 dist = new Vector3(step.X > 0 ? i.X + 1 - position.X : position.X - i.X,
                step.Y > 0 ? i.Y + 1 - position.Y : position.Y - i.Y,
                step.Z > 0 ? i.Z + 1 - position.Z : position.Z - i.Z);
            Vector3 tMax = new Vector3(tDelta.X < float.MaxValue ? tDelta.X * dist.X : float.MaxValue,
                tDelta.Y < float.MaxValue ? tDelta.Y * dist.Y : float.MaxValue,
                tDelta.Z < float.MaxValue ? tDelta.Z * dist.Z : float.MaxValue);
            int steppedIndex = -1;

            while (t <= maxDistance / stepAmount)
            {
                //Console.WriteLine(t);
                bool b = isVoxel(i);

                if (b)
                {
                    hit = new RaycastHit();
                    hit.Position = new Vector3(position.X + t * direction.X, position.Y + t * direction.Y,
                        position.Z + t * direction.Z);
                    hit.Normal = new Vector3(steppedIndex == 0 ? -step.X : 0, steppedIndex == 1 ? -step.Y : 0,
                        steppedIndex == 2 ? -step.Z : 0);

                    return true;
                }

                if (tMax.X < tMax.Y)
                {
                    if (tMax.X < tMax.Z)
                    {
                        i.X += step.X;
                        t = tMax.X;
                        tMax.X += tDelta.X;
                        steppedIndex = 0;
                    }
                    else
                    {
                        i.Z += step.Z;
                        t = tMax.Z;
                        tMax.Z += tDelta.Z;
                        steppedIndex = 2;
                    }
                }
                else
                {
                    if (tMax.Y < tMax.Z)
                    {
                        i.Y += step.Y;
                        t = tMax.Y;
                        tMax.Y += tDelta.Y;
                        steppedIndex = 1;
                    }
                    else
                    {
                        i.Z += step.Z;
                        t = tMax.Z;
                        tMax.Z += tDelta.Z;
                        steppedIndex = 2;
                    }
                }
            }

            hit = new RaycastHit();
            hit.Normal = Vector3.Zero;
            hit.Position = Vector3.Zero;

            return false;
        }
    }

    public struct RaycastHit
    {
        public Vector3 Position;
        public Vector3 Normal;
    }
}