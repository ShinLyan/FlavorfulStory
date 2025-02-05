namespace FlavorfulStory.AI.SceneGraphSystem
{
    public class WarpGraphBuilder
    {
        public static WarpGraph BuildGraph(AllWarpsSetup allWarpsSetup)
        {
            WarpGraph graph = new WarpGraph();

            // Сначала создаем все узлы
            foreach (var sceneWarpsSetup in allWarpsSetup.SceneWarpsSetup)
            {
                int warpIndex = 1; // Индекс варпа внутри сцены (начинаем с 1)

                foreach (var warp in sceneWarpsSetup.warps)
                {
                    // Генерация уникального id на основе sceneType и индекса
                    int id = (int)warp.sceneType * 100 + warpIndex;
                    WarpNode node = new WarpNode(id, warp.sceneType, warp.position);
                    graph.AddNode(node);

                    warpIndex++; // Увеличиваем индекс для следующего варпа
                }
            }

            // Затем добавляем ребра между узлами
            foreach (var sceneWarpsSetup in allWarpsSetup.SceneWarpsSetup)
            {
                int warpIndex = 1; // Индекс варпа внутри сцены (начинаем с 1)

                foreach (var warp in sceneWarpsSetup.warps)
                {
                    // Генерация уникального id для текущего варпа
                    int fromId = (int)warp.sceneType * 100 + warpIndex;

                    foreach (var connectedWarp in warp.connectedWarps)
                    {
                        // Генерация уникального id для целевого варпа
                        int toId = (int)connectedWarp._sceneType * 100 + 1; // Предполагаем, что connectedWarp указывает на первый варп в целевой сцене

                        // Добавляем ребро между узлами
                        graph.AddEdge(fromId, toId, connectedWarp._pathTimeDuration);
                    }

                    warpIndex++; // Увеличиваем индекс для следующего варпа
                }
            }

            return graph;
        }
    }
}