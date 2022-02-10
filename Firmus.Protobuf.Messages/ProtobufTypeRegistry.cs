using Google.Protobuf.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firmus.Protobuf.Messages
{
    public static class ProtobufTypeRegistry
    {
        public static TypeRegistry TypeRegistry = TypeRegistry.FromMessages(new[]{
            Person.Descriptor,
            AddressBook.Descriptor
        });
    }
}
