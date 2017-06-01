using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Windows.Input;

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

            Matrix4 ml = new Matrix4(new Vector4(right, 0),
                                   new Vector4(up, 0),
                                   new Vector4(front, 0),
                                   new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            Matrix4 mr = new Matrix4(new Vector4(1, 0, 0, -position.X),
                                    new Vector4(0, 1, 0, -position.Y),
                                    new Vector4(0, 0, 1, -position.Z),
                                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            viewMatrix = ml * mr;

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
            yaw -= dx * 0.5f;
            pitch += dy * 0.5f;

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

        public void CreateProjection(float n, float f, float fov, float a)
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(DegreeToRadian(fov), a, n, f);
        }

        public Vector3 CastRayThroughPlane(float x, float y, Vector3 planePos)
        {
            Vector4 v = new Vector4(x, y, 0.0f, 1.0f);
            Matrix4 inv = (projectionMatrix * viewMatrix).Inverted();
            v = inv * v;
            v = Vector4.Divide(v, v.W);

            Vector3 rayOrigin = position;
            Vector3 rayDirection = (v.Xyz - rayOrigin).Normalized();
            Vector3 planeNormal = -front;

            float t = Vector3.Dot(planePos - rayOrigin, planeNormal) /
                        Vector3.Dot(rayDirection, planeNormal);

            return rayOrigin + rayDirection * t;
        }

        public void HandleKeyboardMovement(float speed)
        {
            if (Keyboard.IsKeyDown(Key.W))
                position -= front * speed;
            if (Keyboard.IsKeyDown(Key.S))
                position += front * speed;
            if (Keyboard.IsKeyDown(Key.A))
                position -= right * speed;
            if (Keyboard.IsKeyDown(Key.D))
                position += right * speed;
            if (Keyboard.IsKeyDown(Key.LeftShift))
                position -= up * speed;
            if (Keyboard.IsKeyDown(Key.Space))
                position += up * speed;

            Matrix4 ml = new Matrix4(new Vector4(right, 0),
                                    new Vector4(up, 0),
                                    new Vector4(front, 0),
                                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            Matrix4 mr = new Matrix4(new Vector4(1,0,0, -position.X),
                                    new Vector4(0,1,0, -position.Y),
                                    new Vector4(0,0,1, -position.Z),
                                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            viewMatrix = ml * mr;
        }
        #endregion
    }
}
