namespace Sitecore.CustomSerialization.Pipelines
{
    using Sitecore.SecurityModel;
    using System;
    using Sitecore.Diagnostics;

    public abstract class FieldSerializationPipelineProcessor
    {
        public void Process(FieldSerializationPipelineArgs args)
        {
            using (new SecurityDisabler())
            {
                try
                {
                    DoProcess(args);
                }
                catch (Exception ex)
                {
                    Log.Warn(string.Format(
                            "An error occurred during field (de)serialization of item {0} with field {1} and value '{2}'... Falling back to default (de)serializer... Message: {3}",
                            args.Item.ID,
                            args.FieldId,
                            args.ValueNormal ?? args.ValueSerialized,
                            ex.Message)
                        , ex, this);
                    args.FieldSerializationType = FieldSerializationType.Default;
                }
            }
        }

        protected abstract void DoProcess(FieldSerializationPipelineArgs args);
    }
}
