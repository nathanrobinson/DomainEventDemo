using System.IO;
using System.Runtime.Serialization.Json;
using DomainEventDemo.Core.Interfaces;

namespace DomainEventDemo.Services
{
    public class ObjectCloner : IObjectCloner
    {
        public T Clone<T>(T source)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, source);
                ms.Seek(0, 0);
                return (T) serializer.ReadObject(ms);
            }
        }
    }
}