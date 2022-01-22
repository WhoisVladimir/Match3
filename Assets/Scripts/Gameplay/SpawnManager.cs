using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class SpawnManager : Singleton<SpawnManager>
    {
        [SerializeField] private GameObject contentGameObject;
        private List<CellContentObject> contentObjects;

        protected override void Awake()
        {
            base.Awake();
            contentObjects = new List<CellContentObject>();
        }

        public CellContentObject GetContentObject(List<CellContent> content, CellContentObject contentObjectReference)
        {
            if (!contentObjectReference) 
            {
                if (contentObjects.Exists(obj => obj.LocationCell == null))
                    contentObjectReference = contentObjects.Find(obj => obj.LocationCell == null);
                else 
                {
                    contentObjectReference = Instantiate(contentGameObject).GetComponent<CellContentObject>();
                    contentObjects.Add(contentObjectReference);
                }
            }
            else
            {
                if (!contentObjects.Contains(contentObjectReference)) contentObjects.Add(contentObjectReference);
            }
                        
            var contentItemIndex = Random.Range(0, content.Count);
            contentObjectReference.SetObjectContent(content[contentItemIndex]);

            return contentObjectReference;
        }
    }
}
