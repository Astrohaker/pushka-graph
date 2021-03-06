using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PushkaGraph.Core;
using PushkaGraph.Algorithms;
using System.Linq;

namespace PushkaGraph.Tests
{
    [TestClass]
    public class AlgorithmsTests
    {
        [TestMethod]
        public void ExampleExtensionMethodShouldDoNothing()
        {
            var graph = new Graph();

            graph.SomeAlgorithm(null).Should().BeNull();
        }

        [TestMethod]
        public void ConnectedComponents_EmptyGraph()
        {
            var graph = new Graph();
            var connComponentsCount = graph.ConnectedComponentsCount();

            Assert.AreEqual(0, connComponentsCount);
        }

        [TestMethod]
        public void ConnectedComponents_OneVertex()
        {
            var graph = new Graph(1);
            var connComponentsCount = graph.ConnectedComponentsCount();

            Assert.AreEqual(1, connComponentsCount);
        }

        [TestMethod]
        public void ConnectedComponents_ThreeNotСonnectedVertices()
        {
            var graph = new Graph(3);

            var connComponentsCount = graph.ConnectedComponentsCount();
            Assert.AreEqual(3, connComponentsCount);
        }

        [TestMethod]
        public void ConnectedComponents_ThreeVerticesOneComponent()
        {
            var graph = new Graph(3);

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[2]);

            var connComponentsCount = graph.ConnectedComponentsCount();
            Assert.AreEqual(1, connComponentsCount);
        }

        [TestMethod]
        public void ConnectedComponents_ThreeСomponents()
        {
            var graph = new Graph(6);

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1]);
            graph.AddEdge(graph.Vertices[2], graph.Vertices[3]);
            graph.AddEdge(graph.Vertices[2], graph.Vertices[4]);
            graph.AddEdge(graph.Vertices[3], graph.Vertices[4]);

            var connComponentsCount = graph.ConnectedComponentsCount();
            Assert.AreEqual(3, connComponentsCount);
        }

        private void checkEulerianPath(Graph graph, List<Edge> path)
        {
            var selectedEdges = new HashSet<Edge>(path);
            var graphEdges = new HashSet<Edge>(graph.Edges);
            Assert.IsTrue(selectedEdges.SetEquals(graphEdges), 
                "Eulerian path should contain all the graph edges");

            if (path.Count < 2)
                return;

            Vertex adjVertex = null;
            if (path[0].FirstVertex.IncidentEdges.Contains(path[1]))
                adjVertex = path[0].FirstVertex;
            else if (path[0].SecondVertex.IncidentEdges.Contains(path[1]))
                adjVertex = path[0].SecondVertex;
            else
                Assert.Fail("Edge sequence should form correct path");

            for (int i = 1; i < path.Count; ++i)
            {
                Assert.IsTrue(path[i].IsIncidentTo(adjVertex), "Edge sequence should form correct path");
                adjVertex = path[i].GetAdjacentVertexTo(adjVertex);
            }

        }

        [TestMethod]
        public void EulerianPath_GraphAndWithoutOddDegrees_Ok()
        {
            // The test presents the graph from the picture 
            // https://commons.wikimedia.org/wiki/File:Labelled_Eulergraph.svg?uselang=ru

            var graph = new Graph(6);

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[2]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[4]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[5]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[2]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[3]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[4]);
            graph.AddEdge(graph.Vertices[2], graph.Vertices[3]);
            graph.AddEdge(graph.Vertices[2], graph.Vertices[4]);
            graph.AddEdge(graph.Vertices[3], graph.Vertices[4]);
            graph.AddEdge(graph.Vertices[3], graph.Vertices[5]);
            
            checkEulerianPath(graph, new List<Edge>(graph.EulerianPath()));
        }
        
        [TestMethod]
        public void EulerianPath_GraphWithTwoOddDegrees_Ok()
        {
            // The test presents the graph from the picture 
            // https://monographies.ru/images/mono/i3.rae.ru/22/image997.png

            var graph = new Graph(4);

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[2]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[3]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[2]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[3]);
            
            checkEulerianPath(graph, new List<Edge>(graph.EulerianPath()));
        }
        
        [TestMethod]
        public void EulerianPath_OneEdgeGraph_Ok()
        {

            var graph = new Graph(2);

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1]);
            
            checkEulerianPath(graph, new List<Edge>(graph.EulerianPath()));
        }
        
        [TestMethod]
        public void EulerianPath_GraphWithMoreThanTwoOddDegrees_Fail()
        {

            var graph = new Graph(4);

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[2]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[3]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[2]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[3]);
            graph.AddEdge(graph.Vertices[2], graph.Vertices[3]);
            
            Assert.IsNull(graph.EulerianPath(), 
                "There can't be any eulerian path in graph with more than two vertices with odd degrees");
        }
        
        [TestMethod]
        public void EulerianPath_DisconnectedGraphWithIsolatedVertex_Ok()
        {

            var graph = new Graph(4);

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[2]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[2]);
            
            checkEulerianPath(graph, new List<Edge>(graph.EulerianPath()));
        }
        
        [TestMethod]
        public void EulerianPath_DisconnectedGraph_Fail()
        {

            var graph = new Graph(6);

            graph.AddEdge(graph.Vertices[0], graph.Vertices[1]);
            graph.AddEdge(graph.Vertices[0], graph.Vertices[2]);
            graph.AddEdge(graph.Vertices[1], graph.Vertices[2]);
            
            graph.AddEdge(graph.Vertices[3], graph.Vertices[4]);
            graph.AddEdge(graph.Vertices[3], graph.Vertices[5]);
            graph.AddEdge(graph.Vertices[4], graph.Vertices[5]);
            
            Assert.IsNull(graph.EulerianPath(), 
                "There can't be any eulerian path in graph with more than one non-empty components");
        }
        
        [TestMethod]
        public void EulerianPath_EmptyGraph_Ok()
        {

            var graph = new Graph(6);
            
            checkEulerianPath(graph, new List<Edge>(graph.EulerianPath()));
        }
    }
}