using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    using Core;

    /// <summary>
    /// Defines a point in a 2D space.
    /// </summary>
    [System.Serializable]
    public struct CurvePoint
    {
        /// <summary>
        /// The horizontal coordinate.
        /// </summary>
        public float x;

        /// <summary>
        /// The vertical coordinate.
        /// </summary>
        public float y;

        /// <summary>
        /// Creates a new Vector2 struct with the given coordinates.
        /// </summary>
        /// <param name="x">The horizontal coordinate.</param>
        /// <param name="y">The vertical coordinate.</param>
        public CurvePoint(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Add two vectors.
        /// </summary>
        /// <param name="a">The first addend.</param>
        /// <param name="b">The second addend.</param>
        /// <returns>The component sum of the vectors.</returns>
        public static CurvePoint operator +(CurvePoint a, CurvePoint b)
        {
            return new CurvePoint(a.x + b.x, a.y + b.y);
        }

        /// <summary>
        /// Substract two vectors.
        /// </summary>
        /// <param name="a">The first element.</param>
        /// <param name="b">The second element.</param>
        /// <returns>The component substraction of the vectors.</returns>
        public static CurvePoint operator -(CurvePoint a, CurvePoint b)
        {
            return new CurvePoint(a.x - b.x, a.y - b.y);
        }
    }

    /// <summary>
    /// Create a curve factor with an with a linear function defined with points.
    /// </summary>
    public class PointedCurveFactor : CurveFactor
    {
        /// <summary>
        /// The points used to define the function. Must be ordered in its x coord to avoid errors.
        /// </summary>
        public List<CurvePoint> Points = new List<CurvePoint>();

        /// <summary>
        /// Set the points of the pointed curve factor.
        /// </summary>
        /// <param name="points">The new exponent.</param>
        /// <returns>The <see cref="PointedCurveFactor"/> itself. </returns>
        public PointedCurveFactor SetPoints(List<CurvePoint> points)
        {
            Points = points;
            return this;
        }

        /// <summary>
        /// Set the points of the pointed curve factor.
        /// </summary>
        /// <param name="points">The new exponent.</param>
        /// <returns>The <see cref="PointedCurveFactor"/> itself. </returns>
        public PointedCurveFactor SetPoints(params CurvePoint[] points)
        {
            Points = points.ToList();
            return this;
        }


        /// <summary>
        /// Compute the utility using a linear function defined with points.
        /// <para>If x is lower than the first point x coord, the value will be its y coord.</para>
        /// <para>If x is higher than the last point x coord, the value will be its y coord.</para>
        /// </summary>
        /// <param name="x">The child utility. </param>
        /// <returns>The result of apply the function to <paramref name="x"/>.</returns>
        protected override float Evaluate(float x)
        {
            if(Points.Count == 0) return 0;

            int id = FindClosestLowerId(x);

            if (id == -1)
            {
                return Points[0].y;
            }

            else if(id == Points.Count - 1) 
                return Points[Points.Count - 1].y;
            else
            {
                var delta = (x - Points[id].x) / (Points[id + 1].x - Points[id].x);
                return Points[id].y * (1 - delta) + Points[id + 1].y * delta;
            }
        }

        int FindClosestLowerId(float x)
        {
            int id = 0;
            while (id < Points.Count && Points[id].x <= x)
            {
                id++;
            }
            return id - 1;          
        }

        public override object Clone()
        {
            PointedCurveFactor function = (PointedCurveFactor)base.Clone();
            function.Points = Points.ToList();
            return function;
        }
    }
}
