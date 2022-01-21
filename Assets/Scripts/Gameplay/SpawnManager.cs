using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class SpawnManager : Singleton<SpawnManager>
    {
        [SerializeField] private GameObject contentGameObject;

        public CellContentObject GetContentObject(List<CellContent> content, CellContentObject contentObjectReference)
        {
            CellContentObject contentObj;
            if (!contentObjectReference) contentObj = Instantiate(contentGameObject).GetComponent<CellContentObject>();
            else contentObj = contentObjectReference;
            
            var contentItemIndex = Random.Range(0, content.Count);
            contentObj.SetObjectContent(content[contentItemIndex]);

            return contentObj;
        }
    }
}
