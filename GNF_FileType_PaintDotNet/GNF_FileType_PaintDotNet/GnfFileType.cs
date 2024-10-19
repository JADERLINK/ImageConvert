using PaintDotNet;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnfFileType
{
    [PluginSupportInfo(typeof(PluginSupportInfo))]
    public sealed class GnfFileType : PropertyBasedFileType
    {
        private static readonly IReadOnlyList<string> FileExtensions = new string[] { ".gnf" };
        private readonly IServiceProvider services;

        public GnfFileType(IServiceProvider services) :
          base("GNF",
               new FileTypeOptions()
               {
                   LoadExtensions = FileExtensions,
                   SaveExtensions = FileExtensions
               })
        {
            this.services = services;
        }

        public override PropertyCollection OnCreateSavePropertyCollection() 
        {
            List<Property> props = new();
            List<PropertyCollectionRule> rules = new();

            IFileTypeInfo fileTypeInfo = DdsUtils.TryGetFileTypeInfo(services);
            FileType fileType = fileTypeInfo.GetInstance();
            var config = (PropertyBasedSaveConfigToken)fileType.CreateDefaultSaveConfigToken();

            var prop1 = config.GetProperty("ErrorDiffusionDithering");
            var prop2 = config.GetProperty("ErrorMetric");
            props.Add(prop1);
            props.Add(prop2);
            return new PropertyCollection(props, rules);
        }

        public override ControlInfo OnCreateSaveConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultSaveConfigUI(props);
            
            IFileTypeInfo fileTypeInfo = DdsUtils.TryGetFileTypeInfo(services);
            FileType fileType = fileTypeInfo.GetInstance();
            var confw = fileType.CreateSaveConfigWidget();

            var ErrorDiffusionDithering = confw.Controls.Find("checkBox", true).FirstOrDefault()?.Text ?? "Error diffusion dithering";
            var radioButton0 = confw.Controls.Find("radioButton0", true).FirstOrDefault()?.Text ?? "Perceptual";
            var radioButton1 = confw.Controls.Find("radioButton1", true).FirstOrDefault()?.Text ?? "Uniform";
            var ErrorMetric = confw.Controls.Find("radioButton0", true).FirstOrDefault()?.Parent.Controls.Find("header", true).FirstOrDefault()?.Text ?? "Error Metric";

            PropertyControlInfo ditheringPCI = configUI.FindControlForPropertyName("ErrorDiffusionDithering");
            ditheringPCI.ControlProperties[ControlInfoPropertyNames.DisplayName].Value = string.Empty;
            ditheringPCI.ControlProperties[ControlInfoPropertyNames.Description].Value = ErrorDiffusionDithering;

            var prop2 = (StaticListChoiceProperty)props["ErrorMetric"];
            var type = prop2.DefaultValue.GetType();

            PropertyControlInfo errorMetricPCI = configUI.FindControlForPropertyName("ErrorMetric");
            errorMetricPCI.ControlProperties[ControlInfoPropertyNames.DisplayName].Value = ErrorMetric;
            errorMetricPCI.ControlType.Value = PropertyControlType.RadioButton;
            errorMetricPCI.SetValueDisplayName(Enum.Parse(type, "Perceptual"), radioButton0);
            errorMetricPCI.SetValueDisplayName(Enum.Parse(type, "Uniform"), radioButton1);

            return configUI;
        }

        protected override Document OnLoad(Stream input)
        {
            return GnfReader.Load(input, services);
        }

        protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback) 
        {
            GnfWrite.OnSaveT(input, output, token, scratchSurface, progressCallback, services);
        }

    }
}
