using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.AzureOpenAI
{
    public class OpenAIModel{}

    public class Choice
    {
        public int index { get; set; }
        public string finish_reason { get; set; }
        public Message message { get; set; }
    }

    public class Context
    {
        public List<Message> messages { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
        public bool end_turn { get; set; }
        public Context context { get; set; }
    }

    public class RootResponse
    {
        public string id { get; set; }
        public string model { get; set; }
        public int created { get; set; }
        public string @object { get; set; }
        public List<Choice> choices { get; set; }
    }
    public partial class Temperatures
    {
        public Citation[] Citations { get; set; }
        public string Intent { get; set; }
        public string Content { get; set; }
    }

    public partial class Citation
    {
        public string Content { get; set; }
        public object Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public Uri Url { get; set; }
        public Metadata Metadata { get; set; }
        public long ChunkId { get; set; }
    }

    public partial class Metadata
    {
        public string Chunking { get; set; }
    }
}