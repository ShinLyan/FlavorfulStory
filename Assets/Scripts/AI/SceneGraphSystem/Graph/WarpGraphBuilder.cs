using System.Collections.Generic;
using FlavorfulStory.SceneManagement;
using UnityEngine;

namespace FlavorfulStory.AI.SceneGraphSystem
{
    /// <summary> Класс для построения графа варпов. </summary>
    public static class WarpGraphBuilder
    {
        /// <summary> Строит граф варпов на основе списка всех варпов. </summary>
        /// <param name="allWarps"> Список всех варпов для построения графа. </param>
        /// <returns> Построенный граф варпов. </returns>
        public static WarpGraph BuildGraph(IEnumerable<Warp> allWarps)
        {
            var graph = new WarpGraph();
            var nodeMap = new Dictionary<Warp, WarpNode>();
            var warpsByLocation = new Dictionary<LocationType, List<Warp>>();

            // Группируем варпы по локациям
            foreach (var warp in allWarps)
            {
                if (!warpsByLocation.ContainsKey(warp.ParentLocation))
                    warpsByLocation[warp.ParentLocation] = new List<Warp>();
                
                warpsByLocation[warp.ParentLocation].Add(warp);
            }

            // Создаем узлы и связи внутри локаций
            foreach (var location in warpsByLocation.Keys)
            {
                var warpsInLocation = warpsByLocation[location];
        
                foreach (var warp in warpsInLocation)
                {
                    var node = new WarpNode(warp);
                    nodeMap[warp] = node;
                    graph.AddNode(node);
                }

                // Связываем все варпы внутри локации между собой
                foreach (var warpA in warpsInLocation)
                {
                    foreach (var warpB in warpsInLocation)
                    {
                        if (warpA != warpB)
                        {
                            float distance = Vector3.Distance(warpA.transform.position, warpB.transform.position);
                            nodeMap[warpA].Edges.Add(new WarpEdge(nodeMap[warpB], Mathf.RoundToInt(distance)));
                        }
                    }
                }
            }

            // Добавляем связи между варпами разных локаций
            foreach (var warp in allWarps)
                foreach (var connectedWarp in warp.ConnectedWarps)
                    if (nodeMap.TryGetValue(connectedWarp, out var targetNode))
                        nodeMap[warp].Edges.Add(new WarpEdge(targetNode, warp.TransitionDuration));

            return graph;
        }
    }
}