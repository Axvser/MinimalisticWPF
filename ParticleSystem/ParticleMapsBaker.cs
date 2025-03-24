using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimalisticWPF.ParticleSystem
{
    public sealed class ParticleMapsBaker(ParticleCanvas canvas)
    {
        private readonly Random randomIndex = new();
        private readonly Random randomPosition = new();
        private readonly Random randomDuration = new();

        private readonly double randomDurationDelta = canvas.DurationObfuscationConstant;
        private readonly double duration = canvas.Duration;

        private readonly ParticleCanvas canvas = canvas;
        private IEnumerable<ParticleMap> maps = [];
        private bool isActived = false;
        private bool isVisible = false;
        private readonly ConcurrentDictionary<Shape, ShapeEdgeSampler> samplers = [];
        private readonly int particleCount = canvas.Capacity;

        private readonly Pen startPen = new(canvas.StartBrush, canvas.StartSize);
        private readonly Pen endPen = new(canvas.EndBrush, canvas.EndSize);

        public bool IsActived => isActived;
        public bool IsVisible => isVisible;

        public IEnumerable<Tuple<Point, Pen>> ReadSource()
        {
            return maps.Select(state => Tuple.Create(
                       state.StateValue.Position,
                       state.StateValue.Pen));
        }

        public void UpdateSamplers()
        {
            var shapes = canvas.Children.OfType<Shape>().ToList();

            if (shapes.Count < 2)
            {
                Clear();
                return;
            }

            foreach (var shape in shapes)
            {
                if (samplers.TryGetValue(shape, out var sampler))
                {
                    samplers.TryUpdate(shape, new ShapeEdgeSampler(shape, canvas), sampler);
                }
                else
                {
                    samplers.TryAdd(shape, new ShapeEdgeSampler(shape, canvas));
                }
            }
        }

        public async Task UpdateMaps()
        {
            Clear();

            var list = new List<ParticleMap>(particleCount);

            var shapes = canvas.Children.OfType<Shape>().ToList();

            if (shapes.Count < 2) return;

            var reciever = shapes.Last();
            var emitters = shapes.Take(shapes.Count - 1).ToList();

            for (int i = 0; i < particleCount; i++)
            {
                if (samplers.TryGetValue(emitters[randomIndex.Next(0, emitters.Count)], out var emitterSampler)
                    && samplers.TryGetValue(reciever, out var recieverSampler))
                {
                    var emitterPosition = emitterSampler.GetRandomPointOnEdge(randomPosition);
                    var recieverPosition = recieverSampler.GetRandomPointOnEdge(randomPosition);
                    var map = ParticleMap.From(emitterPosition, startPen).To(recieverPosition, endPen);
                    map.Duration = duration + randomDuration.NextDouble() * randomDurationDelta;
                    list.Add(map);
                }
            }
            maps = list;

            await Start();
        }

        public async Task Start()
        {
            isVisible = false;
            foreach (var map in maps)
            {
                await map.Start();
            }
            isActived = true;
            await Task.Delay((int)((duration + randomDurationDelta) * 1000));
            isVisible = true;
        }

        public void Stop()
        {
            foreach (var map in maps)
            {
                map.Stop();
            }
            isActived = false;
        }

        public void Clear()
        {
            Stop();
            maps = [];
            isActived = false;
        }
    }
}
