using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ClassLibrary1
{
    public static class FuncAux
    {
        public static List<CustomRevitLink> GetRevitLinks(Document doc)
        {
            List<CustomRevitLink> revitLinks = new List<CustomRevitLink>();

            // Obtém todos os elementos de vínculo Revit
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> revitLinkElements = collector.OfClass(typeof(RevitLinkType)).ToElements();

            foreach (Element element in revitLinkElements)
            {
                RevitLinkType linkType = element as RevitLinkType;
                if (linkType != null && !linkType.IsNestedLink)
                {
                    // Obtém o nome do vínculo Revit
                    string linkName = linkType.Name;

                    // Cria um objeto RevitLink com o nome e o tipo de vínculo
                    CustomRevitLink revitLink = new CustomRevitLink(linkName, linkType);
                    revitLinks.Add(revitLink);
                }
            }

            return revitLinks;
        }

        /*
         * Consegui desenvolver o método para acessar a vista desejada, porém nao utilizei pois não consegui fazer a parte dos métodos de interseção
         */
        public static View GetVistaTeste(Document document)
        {
            string vistaTesteName = "Vista teste"; // Nome da vista desejada

            // Obtém todas as vistas do documento
            FilteredElementCollector viewCollector = new FilteredElementCollector(document);
            ICollection<Element> views = viewCollector.OfClass(typeof(View)).ToElements();

            // Procura a vista com o nome "Vista teste"
            foreach (Element element in views)
            {
                View view = element as View;
                if (view != null && view.Name == vistaTesteName)
                {
                    return view;
                }
            }

            return null; // Se a vista não for encontrada
        }

        //tentativa 2

        public static List<Element> ColetarElementosTubos(Document doc, ElementId linkInstanceId)
        {
            List<ElementId> tuboIds = new List<ElementId>()
            {
                new ElementId(250505), // ID do primeiro tubo
                new ElementId(249809) // ID do segundo tubo
            };

            List<Element> tubosEncontrados = new List<Element>();

            // Obter o arquivo vinculado dos tubos
            RevitLinkInstance tubosLinkInstance = GetRevitLinkInstanceById(doc, linkInstanceId);
            if (tubosLinkInstance != null)
            {
                Document tubosLinkDocument = tubosLinkInstance.GetLinkDocument();

                // Procurar elementos de tubos pelo ID no arquivo vinculado dos tubos
                foreach (ElementId tuboId in tuboIds)
                {
                    Element tubo = tubosLinkDocument.GetElement(tuboId);
                    if (tubo != null)
                    {
                        tubosEncontrados.Add(tubo);
                    }
                }
            }

            return tubosEncontrados;
        }

        public static List<Element> ColetarElementosLajes(Document doc, ElementId linkInstanceId)
        {
            List<ElementId> lajeIds = new List<ElementId>()
    {
        new ElementId(208221) // ID da laje
    };

            List<Element> lajesEncontradas = new List<Element>();

            // Obter o arquivo vinculado da laje

            RevitLinkInstance lajeLinkInstance = GetRevitLinkInstanceById(doc, linkInstanceId);

            if (lajeLinkInstance != null)
            {
                Document lajeLinkDocument = lajeLinkInstance.GetLinkDocument();

                // Procurar elementos de laje pelo ID no arquivo vinculado da laje
                foreach (ElementId lajeId in lajeIds)
                {
                    Element laje = lajeLinkDocument.GetElement(lajeId);
                    if (laje != null)
                    {
                        lajesEncontradas.Add(laje);
                    }
                }
            }

            return lajesEncontradas;
        }

        public static RevitLinkInstance GetRevitLinkInstanceById(Document doc, ElementId linkInstanceId)
        {
            FilteredElementCollector linkInstancesCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance));

            RevitLinkInstance linkInstance = linkInstancesCollector
                .WhereElementIsNotElementType()
                .FirstOrDefault(x => x.Id == linkInstanceId) as RevitLinkInstance;

            return linkInstance;
        }


        //Primeira tentativa para encontrar os "intersect points" entre os tubos e as lajes

        /*
         * Esse método consistia em transformar o tubo em uma linha e a laje em um plano.
         * Pórem quando eu tentava criar um método que retornava um list<XYZ> que percorria um List<Line> e um list<Plane> sempre chegava em um erro.
         * Por conta do erro, apaguei essa parte do código que encontrava os "intersect points" para ele conseguir ser compilado.
        */

        public static Line GetVerticalLine(Element tubo)
        {
            LocationCurve locationCurve = tubo.Location as LocationCurve;
            if (locationCurve != null)
            {
                Curve curve = locationCurve.Curve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                // Verificar qual ponto está mais alto
                XYZ highestPoint = startPoint.Y > endPoint.Y ? startPoint : endPoint;
                XYZ lowestPoint = startPoint.Y > endPoint.Y ? endPoint : startPoint;

                // Criar e retornar a linha vertical
                return Line.CreateBound(highestPoint, lowestPoint);
            }

            return null;
        }
        public static Plane GetPlaneFromTopEdges(Element laje)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;

            GeometryElement geometryElement = laje.get_Geometry(options);
            foreach (GeometryObject geomObject in geometryElement)
            {
                if (geomObject is Solid solid)
                {
                    FaceArray faces = solid.Faces;

                    // Filtrar as faces superiores
                    List<Face> topFaces = new List<Face>();
                    foreach (Face face in faces)
                    {
                        if (IsTopFace(face))
                        {
                            topFaces.Add(face);
                        }
                    }

                    // Verificar se há pelo menos 4 faces superiores
                    if (topFaces.Count >= 4)
                    {
                        // Extrair os pontos das arestas das faces superiores
                        List<XYZ> edgePoints = new List<XYZ>();
                        foreach (Face face in topFaces)
                        {
                            EdgeArrayArray edgeLoops = face.EdgeLoops;
                            foreach (EdgeArray edgeLoop in edgeLoops)
                            {
                                foreach (Edge edge in edgeLoop)
                                {
                                    XYZ startPoint = edge.Evaluate(0);
                                    XYZ endPoint = edge.Evaluate(1);
                                    edgePoints.Add(startPoint);
                                    edgePoints.Add(endPoint);
                                }
                            }
                        }

                        // Criar um plano usando os 4 primeiros pontos das arestas superiores
                        if (edgePoints.Count >= 4)
                        {
                            XYZ firstPoint = edgePoints[0];
                            XYZ secondPoint = edgePoints[1];
                            XYZ thirdPoint = edgePoints[2];
                            XYZ fourthPoint = edgePoints[3];
                            return Plane.CreateByThreePoints(firstPoint, secondPoint, fourthPoint);
                        }
                    }
                }
            }

            return null;
        }

        private static bool IsTopFace(Face face)
        {
            XYZ faceNormal = face.ComputeNormal(new UV());
            return faceNormal.IsAlmostEqualTo(XYZ.BasisZ);
        }

        //Segunda tentativa 

        /*
         *Esse segundo método consistia em transformar os elementos em sólidos e encontrar as partes em que os sólidos se interceptavam.
         *Esse código estava funcional até a parte de tentar adicionar uma familia 'FURO-QUADRADO-LAJE'
         *Não consegui desenvolver uma lógica que compilasse para encaixar a família no ponto que encontrei.
         *Não sei se foi por conta do método que utilizei para encontrar o XYZ ou porque a lógica para a adição da familia não estava correta.
         *Como essa parte não estava compilando, exclui a lógica para adicionar a familia 'FURO-QUADRADO-LAJE'
        */

        public static Solid GetIntersectionSolid(Element element1, Element element2)
        {
            GeometryElement geometry1 = element1.get_Geometry(new Options());
            GeometryElement geometry2 = element2.get_Geometry(new Options());

            Solid solid1 = GetSolidFromGeometry(geometry1);
            Solid solid2 = GetSolidFromGeometry(geometry2);

            if (solid1 != null && solid2 != null)
            {
                Solid intersectionSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
                return intersectionSolid;
            }

            return null;
        }

        private static Solid GetSolidFromGeometry(GeometryElement geometryElement)
        {
            foreach (GeometryObject geometryObj in geometryElement)
            {
                if (geometryObj is Solid solid)
                {
                    return solid;
                }
            }

            return null;
        }
        private static XYZ GetIntersectionSolidCenter(Solid solid)
        {
            BoundingBoxXYZ boundingBox = solid.GetBoundingBox();
            XYZ center = 0.5 * (boundingBox.Min + boundingBox.Max);
            return center;
        }

    }
}
