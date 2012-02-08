using System;
using PostSharp.Aspects.Configuration;
using PostSharp.Extensibility;
using PostSharp.Sdk.AspectWeaver;
using PostSharp.Sdk.AspectWeaver.AspectWeavers;
using PostSharp.Sdk.CodeModel;

namespace PostSharp.Toolkits.Weavers
{
    public sealed class LoggingAspectWeaver : MethodLevelAspectWeaver
    {
        private static readonly AspectConfigurationAttribute defaultConfiguration = new MethodInterceptionAspectConfigurationAttribute();
        private LoggingAspectTransformation transformation;

        public LoggingAspectWeaver()
            : base(defaultConfiguration, MulticastTargets.Property | MulticastTargets.Method | MulticastTargets.Class)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.transformation = new LoggingAspectTransformation(this);

            ApplyWaivedEffects(this.transformation);
        }

        protected override AspectWeaverInstance CreateAspectWeaverInstance(AspectInstanceInfo aspectInstanceInfo)
        {
            return new LoggingAspectWeaverInstance(this, aspectInstanceInfo);
        }

        private class LoggingAspectWeaverInstance : MethodLevelAspectWeaverInstance
        {
            public LoggingAspectWeaverInstance(MethodLevelAspectWeaver aspectWeaver, AspectInstanceInfo aspectInstanceInfo)
                : base(aspectWeaver, aspectInstanceInfo)
            {
            }

            public override void ProvideAspectTransformations(AspectWeaverTransformationAdder adder)
            {
                LoggingAspectTransformation transformation = ((LoggingAspectWeaver)AspectWeaver).transformation;
                AspectWeaverTransformationInstance transformationInstance = transformation.CreateInstance(this);

                PropertyDeclaration property = TargetElement as PropertyDeclaration;
                if (property != null)
                {
                    MethodDefDeclaration setter = property.Setter;
                    if (setter != null)
                    {
                        adder.Add(setter, transformationInstance);
                    }
                    MethodDefDeclaration getter = property.Getter;
                    if (getter != null)
                    {
                        adder.Add(getter, transformationInstance);
                    }
                    return;
                }

                adder.Add(TargetElement, transformationInstance);
            }
        }

    }
}