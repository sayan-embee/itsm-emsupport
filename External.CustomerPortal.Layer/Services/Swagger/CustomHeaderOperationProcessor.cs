using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace External.CustomerPortal.Layer.Services.Swagger
{
    public class CustomHeaderOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            // Add the custom header as a parameter to the operation
            context.OperationDescription.Operation.Parameters.Add(new NSwag.OpenApiParameter
            {
                Name = "api_key", // Replace with actual header name
                Description = "Custom header for API requests, configurable in Swagger UI",
                Kind = NSwag.OpenApiParameterKind.Header,
                Type = NJsonSchema.JsonObjectType.String, // Header type
                IsRequired = false // Set true if mandatory
            });

            return true;
        }
    }
}
