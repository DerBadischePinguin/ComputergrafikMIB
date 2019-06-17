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
    public class AssetsPicking : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private TransformComponent _baseTransform;
        private float _camAngle = 0;
        private TransformComponent _WheelVLTransform;
        private TransformComponent _WheelVRTransform;
        private TransformComponent _WheelHLTransform;
        private TransformComponent _WheelHRTransform;
        private ShaderEffectComponent _WheelVLShader;
        private ScenePicker _scenePicker;
        private PickResult _currentPick;
        private float3 _oldColor;
        private TransformComponent _ArmTransform;
        private TransformComponent _BodyTransform;
        private TransformComponent _SchaufelTransform;

        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _baseTransform = new TransformComponent
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

           

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Components = new List<SceneComponentContainer>
                        {
                            // TRANSFROM COMPONENT
                            _baseTransform,

                            // SHADER EFFECT COMPONENT
                            new ShaderEffectComponent
                            {
                                Effect = SimpleMeshes.MakeShaderEffect(new float3(0.7f, 0.7f, 0.7f), new float3(1, 1, 1), 5)
                            },

                            // MESH COMPONENT
                            //SimpleAssetsPickinges.CreateCuboid(new float3(10, 10, 10)),
                            SimpleMeshes.CreateCuboid(new float3(10, 10, 10))
                        }
                    },
                }
            };
        }

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

           _scene = AssetStorage.Get<SceneContainer>("Bagger.fus");

        _WheelVLTransform = _scene.Children.FindNodes(node => node.Name == "WheelVL")?.FirstOrDefault()?.GetTransform();
        _WheelVRTransform = _scene.Children.FindNodes(node => node.Name == "WheelVR")?.FirstOrDefault()?.GetTransform();
        _WheelHLTransform = _scene.Children.FindNodes(node => node.Name == "WheelHL")?.FirstOrDefault()?.GetTransform();
        _WheelHRTransform = _scene.Children.FindNodes(node => node.Name == "WheelHR")?.FirstOrDefault()?.GetTransform();
        _ArmTransform = _scene.Children.FindNodes(node => node.Name == "Arm")?.FirstOrDefault()?.GetTransform();
        _SchaufelTransform = _scene.Children.FindNodes(node => node.Name == "Schaufel")?.FirstOrDefault()?.GetTransform();
        _BodyTransform = _scene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault()?.GetTransform();
       
        _WheelVLShader = _scene.Children.FindNodes(node => node.Name == "WheelVL")?.FirstOrDefault()?.GetComponent<ShaderEffectComponent>();
        //_WheelVLShader.SetEffectParam("DiffuseColor", new float3(1, 0.4f, 0.4f));

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
            _scenePicker = new ScenePicker(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
           // _baseTransform.Rotation = new float3(0, M.MinAngle(TimeSinceStart), 0);
           // _WheelVLTransform.Rotation = new float3(M.MinAngle(TimeSinceStart), 0, 0);

           
           
           

            
           // _SchaufelTransform.Scale = new float3(0.5f, 2f, 2f);
           

            

            if (Mouse.MiddleButton == true)
            {
                _camAngle = _camAngle + Mouse.Velocity.x /500;
            } ;

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

           

            // Setup the camera 
            //RC.View = float4x4.CreateTranslation(0, -10, 50) * float4x4.CreateRotationY(_camAngle);
            RC.View = float4x4.CreateTranslation(0, 0, 40) * float4x4.CreateRotationY(_camAngle);


            if (Mouse.LeftButton)
            {
                float2 pickPosClip = Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);
                _scenePicker.View = RC.View;
                _scenePicker.Projection = RC.Projection;

                List<PickResult> pickResults = _scenePicker.Pick(pickPosClip).ToList();
                PickResult newPick = null;
                if (pickResults.Count > 0)
                {
                    pickResults.Sort((a, b) => Sign(a.ClipPos.z - b.ClipPos.z));
                    newPick = pickResults[0];
                }

                if (newPick?.Node != _currentPick?.Node)
                {
                    if (_currentPick != null)
                    {
                        ShaderEffectComponent shaderEffectComponent = _currentPick.Node.GetComponent<ShaderEffectComponent>();
                        shaderEffectComponent.Effect.SetEffectParam("DiffuseColor", _oldColor);
                    }
                    if (newPick != null)
                    {
                        ShaderEffectComponent shaderEffectComponent = newPick.Node.GetComponent<ShaderEffectComponent>();
                        _oldColor = (float3)shaderEffectComponent.Effect.GetEffectParam("DiffuseColor");
                        shaderEffectComponent.Effect.SetEffectParam("DiffuseColor", new float3(0f, 1f, 0.1f));
                    }
                    _currentPick = newPick;
                }
            }

             //Wheel
             if(_currentPick?.Node.Name=="WheelVL" ||_currentPick?.Node.Name=="WheelHL"||_currentPick?.Node.Name=="WheelVR"||_currentPick?.Node.Name=="WheelHR" ){
            float VL = _WheelVLTransform.Rotation.z;
            VL += 0.1f * Keyboard.LeftRightAxis;
            _WheelVLTransform.Rotation = new float3(0, 0, VL );
            float HL = _WheelHLTransform.Rotation.z;
            HL += 0.1f * Keyboard.LeftRightAxis;
            _WheelHLTransform.Rotation = new float3(0, 0 , HL );
            float VR = _WheelVRTransform.Rotation.z;
            VR += 0.1f * Keyboard.LeftRightAxis;
            _WheelVRTransform.Rotation = new float3(0, 0 , VR );
            float HR = _WheelHRTransform.Rotation.z;
            HR += 0.1f * Keyboard.LeftRightAxis;
            _WheelHRTransform.Rotation = new float3(0, 0, HR );
             }

            //Body
            if(_currentPick?.Node.Name=="Body"){
              
            float Body = _BodyTransform.Rotation.y;
            Body += 0.1f * Keyboard.LeftRightAxis;
            _BodyTransform.Rotation = new float3( 0, Body, 0);  
            }

            //Arm
            if(_currentPick?.Node.Name=="Arm"){
            float Arm = _ArmTransform.Rotation.z;
           Arm += 0.1f * Keyboard.LeftRightAxis;
            _ArmTransform.Rotation = new float3(0, 0, Arm);
            }

             //Schaufel
             if(_currentPick?.Node.Name=="Schaufel"){
            float Schaufel = _SchaufelTransform.Rotation.z;
           Schaufel += 0.1f * Keyboard.UpDownAxis;
            _SchaufelTransform.Rotation = new float3(0, 0, Schaufel);
             }

            

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

            // 0.25*PI Rad -> 45� Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}