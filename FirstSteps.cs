using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Tutorial.Core
{
    public class FirstSteps : RenderCanvas
    {

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private float _camAngle = 0;
        private TransformComponent _cubeTransform;
        private ShaderEffectComponent _cubeShader;
        private TransformComponent _cube2Transform;
        private ShaderEffectComponent _cube2Shader;
        private TransformComponent _cube3Transform;
        private ShaderEffectComponent _cube3Shader;
        
        // Init is called on startup. 
        public override void Init()
    {
            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = ColorUint.Tofloat4(ColorUint.PaleGreen);

            // Create a scene with a cube
            // The three components: one XForm, one Shader and the Mesh
             _cubeTransform = new TransformComponent {Scale = new float3(1, 1, 1 ), Translation = new float3(0, 0, 0), Rotation = new float3 (0.1f, 0, 0.3f)};
            _cubeShader = new ShaderEffectComponent
            { 
             Effect = SimpleMeshes.MakeShaderEffect(ColorUint.Tofloat3(ColorUint.Crimson), new float3 (1, 1, 1),  4)
            };
            var cubeMesh = SimpleMeshes.CreateCuboid(new float3(10, 10, 10));

            // Assemble the cube node containing the three components
            var cubeNode = new SceneNodeContainer();
             cubeNode.Components = new List<SceneComponentContainer>();
             cubeNode.Components.Add(_cubeTransform);
             cubeNode.Components.Add(_cubeShader);
             cubeNode.Components.Add(cubeMesh);
// Würfel 2
         _cube2Transform = new TransformComponent {Scale = new float3(5, 1, 1 ), Translation = new float3 (30, 0, 30), Rotation = new float3 (0.6f, 0.6f, 0)};
         _cube2Shader = new ShaderEffectComponent
        { 
          Effect = SimpleMeshes.MakeShaderEffect(ColorUint.Tofloat3(ColorUint.Navy), new float3 (1, 1, 1),  4)
        };
         var cubeMesh2 = SimpleMeshes.CreateCuboid(new float3(5, 5, 5));

         // Assemble the cube node containing the three components
        var cubeNode2 = new SceneNodeContainer();
         cubeNode2.Components = new List<SceneComponentContainer>();
         cubeNode2.Components.Add(_cube2Transform);
         cubeNode2.Components.Add(_cube2Shader);
         cubeNode2.Components.Add(cubeMesh2);
//würfel 3
         _cube3Transform = new TransformComponent {Scale = new float3(1, 1, 1 ), Translation = new float3(18, 0, -30)};
         _cube3Shader = new ShaderEffectComponent
        { 
          Effect = SimpleMeshes.MakeShaderEffect(ColorUint.Tofloat3(ColorUint.Lime), new float3 (1, 2, 1),  4)
        };
         var cubeMesh3 = SimpleMeshes.CreateCuboid(new float3(1, 1, 1));

        // Assemble the cube node containing the three components
        var cubeNode3 = new SceneNodeContainer();
         cubeNode3.Components = new List<SceneComponentContainer>();
         cubeNode3.Components.Add(_cube3Transform);
         cubeNode3.Components.Add(_cube3Shader);
         cubeNode3.Components.Add(cubeMesh3);

        // Create the scene containing the cube as the only object
        _scene = new SceneContainer();
        _scene.Children = new List<SceneNodeContainer>();
        _scene.Children.Add(cubeNode);
        _scene.Children.Add(cubeNode2);
        _scene.Children.Add(cubeNode3);

        // Create a scene renderer holding the scene above
        _sceneRenderer = new SceneRenderer(_scene);

        // RenderAFrame is called once a frame
    }
        
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            // Animate the camera angle
            _camAngle = _camAngle + 90.0f * M.Pi/180.0f * DeltaTime;
             // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 50) * float4x4.CreateRotationY(_camAngle);
             // Animate the cube
             _cubeTransform.Translation = new float3(0, 5 * M.Sin(3 * TimeSinceStart), 0);
            _cubeTransform.Rotation = new float3(TimeSinceStart, TimeSinceStart/2, 0);
            _cube2Transform.Scale = new float3(TimeSinceStart/5, 5, 3);
            _cube2Transform.Rotation = new float3 (0, TimeSinceStart*2, TimeSinceStart/2);
             _cube3Transform.Translation = new float3(0, 9 * M.Sin(8 * TimeSinceStart), 0);

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}