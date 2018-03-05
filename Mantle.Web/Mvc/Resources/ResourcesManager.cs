//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Mantle.Web.Mvc.Resources
//{
//    public class ResourcesManager : IResourcesManager
//    {
//        private readonly Dictionary<string, MetaEntry> metas = new Dictionary<string, MetaEntry>();

//        #region IResourceManager Members

//        public void SetMeta(MetaEntry meta)
//        {
//            metas[meta.Name ?? Guid.NewGuid().ToString()] = meta;
//        }

//        public void AppendMeta(MetaEntry meta, string contentSeparator)
//        {
//            if (meta == null || String.IsNullOrEmpty(meta.Name))
//            {
//                return;
//            }

//            MetaEntry existingMeta;
//            if (metas.TryGetValue(meta.Name, out existingMeta))
//            {
//                meta = MetaEntry.Combine(existingMeta, meta, contentSeparator);
//            }
//            metas[meta.Name] = meta;
//        }

//        public virtual IEnumerable<MetaEntry> GetRegisteredMetas()
//        {
//            return metas.Values.ToList().AsReadOnly();
//        }

//        #endregion IResourceManager Members
//    }
//}