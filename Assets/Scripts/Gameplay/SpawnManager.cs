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

        public CellContentObject GetContentObject(List<CellContent> content, CellContentObject contentObj = null)
        {
            if (contentObj == null) 
            {

                if (contentObjects.Exists(obj => obj.gameObject.activeInHierarchy == false))
                    contentObj = contentObjects.Find(obj => obj.gameObject.activeInHierarchy == false);
                else 
                {
                    if (contentObjects.Count >= 30) return null;

                    contentObj = Instantiate(contentGameObject).GetComponent<CellContentObject>();
                    contentObjects.Add(contentObj);
                }
            }
                        
            var contentItemIndex = Random.Range(0, content.Count);
            contentObj.SetObjectContent(content[contentItemIndex]);

            return contentObj;
        }
    }
}
