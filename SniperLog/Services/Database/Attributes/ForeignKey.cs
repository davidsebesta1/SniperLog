using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLog.Services.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKey : Attribute
    {
        public Type ReferencedClass { get; }
        public string PropertyName { get; }

        public ForeignKey(Type refClass, string propertyName)
        {
            this.ReferencedClass = refClass;
            this.PropertyName = propertyName;
        }
    }
}