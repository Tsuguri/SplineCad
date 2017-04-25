using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SplineCAD.Rendering
{
    public class Camera
    {

        #region Fields

        private Vector3 position;
        private Vector3 front;
        private Vector3 up;
        private Vector3 right;

        private float pitch; //in degrees
        private float yaw; //in degrees

        private Matrix4 viewMatrix;
        private Matrix4 projectionMatrix;

        private readonly Vector3 upWorld = new Vector3(0.0f, 1.0f, 0.0f);

        #endregion

        #region Properties
        public Vector3 Position
        {
            get => position;
            set
            {
                if (position == value)
                    return;
                position = value;
            }
        }
        public Vector3 Front { get => front; }
        public Vector3 Up { get => up; }
        public Vector3 Right { get => right; }

        public float Pitch { get => pitch; }
        public float Yaw { get => yaw; }

        public Matrix4 ViewMatrix { get => viewMatrix; }
        public Matrix4 ProjectionMatrix { get => projectionMatrix; }
        #endregion

        #region Constructors

        public Camera() { }

        public Camera(Vector3 pos)
        {
            position = pos;
            front = new Vector3(0.0f, 0.0f, 1.0f);
            up = new Vector3(0.0f, 1.0f, 0.0f);
            right = new Vector3(1.0f, 0.0f, 0.0f);
            
            pitch = 0.0f;
            yaw = 90.0f;
            
            viewMatrix = new Matrix4(new Vector4(right, -position.X),
                                    new Vector4(up, -position.Y),
                                    new Vector4(front, -position.Z),
                                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI / 4.0),
                1.0f, 1.0f, 100.0f);
        }
        #endregion

        #region Methods

        private float DegreeToRadian(float angle)
        {
            return (float)(Math.PI * angle / 180.0);
        }

        public void Rotate(float dx, float dy)
        {
            yaw -= dx;
            pitch += dy;

            if (pitch > 89.0f) pitch = 89.0f;
            if (pitch < -89.0f) pitch = -89.0f;

            front.X = (float)(Math.Cos(DegreeToRadian(yaw)) *
                      Math.Cos(DegreeToRadian(pitch)));
            front.Y = (float)Math.Sin(DegreeToRadian(pitch));
            front.Z = (float)(Math.Sin(DegreeToRadian(yaw)) *
                      Math.Cos(DegreeToRadian(pitch)));
            front.Normalize();

            right = Vector3.Cross(-front, upWorld);
            right.Normalize();
            up = Vector3.Cross(right, -front);
            up.Normalize();

            viewMatrix.Row0.Xyz = right;
            viewMatrix.Row1.Xyz = up;
            viewMatrix.Row2.Xyz = front;
        }

        public void Zoom(float d)
        {
            position += position * d;

            viewMatrix.Row0.W = position.X;
            viewMatrix.Row1.W = position.Y;
            viewMatrix.Row2.W = position.Z;
        }

        public void CreateProjection(float n, float f, float fov, float a)
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fov, a, n, f);
        }
        #endregion
    }
}
