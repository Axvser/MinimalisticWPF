using MinimalisticWPF.StructuralDesign.Animator;
using MinimalisticWPF.TransitionSystem.Basic;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.ParticleSystem
{
    public sealed class ParticleState(Point position, Pen pen) : IInterpolable
    {
        public static ParticleState Default { get; } = new ParticleState(new Point(0, 0), new Pen(Brushes.Transparent, 0));

        public Point Position { get; set; } = position;
        public Pen Pen { get; set; } = pen;

        public ParticleState Clone()
        {
            return new ParticleState(new Point(Position.X, Position.Y), new Pen(Pen.Brush.Clone(), Pen.Thickness));
        }

        public List<object?> Interpolate(object? current, object? target, int steps)
        {
            if (current is not ParticleState start || target is not ParticleState end)
            {
                throw new ArgumentException("Both current and target must be of type ParticleState");
            }

            if (steps < 1)
            {
                return [end];
            }

            // 预分配列表容量，避免频繁扩容
            var result = new List<object?>(steps);

            // 提前计算插值结果
            var positions = LinearInterpolation.PointComputing(start.Position, end.Position, steps);
            var brushes = LinearInterpolation.BrushComputing(start.Pen.Brush, end.Pen.Brush, steps);
            var thicknesses = LinearInterpolation.DoubleComputing(start.Pen.Thickness, end.Pen.Thickness, steps);

            // 插值逻辑
            for (int i = 0; i < steps; i++)
            {
#pragma warning disable CS8605 
                var position = (Point)positions[i];
#pragma warning restore CS8605 
#pragma warning disable CS8600
                var brush = (Brush)brushes[i];
#pragma warning restore CS8600
#pragma warning disable CS8605 
                var thickness = (double)thicknesses[i];
#pragma warning restore CS8605 

                result.Add(new ParticleState(position, new Pen(brush, thickness)));
            }

            // 确保首尾值准确
            if (steps > 1)
            {
                result[0] = start.Clone();
                result[steps - 1] = end.Clone();
            }

            return result;
        }
    }
}
