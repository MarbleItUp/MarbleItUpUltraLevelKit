using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;

namespace MIU.ProBuilder
{
    [ProBuilderMenuAction]
    sealed class QuadUVStrip : MenuAction
    {
        public override ToolbarGroup group
        {
            get { return ToolbarGroup.Selection; }
        }

        public override Texture2D icon
        {
            get { return IconUtility.GetIcon("Toolbar/Panel_Smoothing"); }
        }

        public override TooltipContent tooltip
        {
            get { return s_Tooltip; }
        }

        static readonly TooltipContent s_Tooltip = new TooltipContent
            (
                "Quad UV Strip",
                "Creates a UV strip from the selected quads taking into account the approximate length of each quad."
            );

        public override SelectMode validSelectModes
        {
            get { return SelectMode.Face | SelectMode.TextureFace; }
        }

        public override bool enabled
        {
            get { return base.enabled && MeshSelection.selectedFaceCount > 0; }
        }

        public override ActionResult DoAction()
        {
            ProBuilderMesh[] selected = Selection.GetFiltered<ProBuilderMesh>(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);
            Undo.RecordObjects(selected, "Quad UV Strip");
            var facesValid = 0;
            var facesInvalid = 0;
            var updatedFaces = new List<Face>();
            var uv = new List<Vector4>();
            for(int i = 0; i < selected.Length; i++)
            {
                var obj = selected[i];
                var vertices = obj.positions;
                obj.GetUVs(0, uv);
                var selectedFaces = obj.GetSelectedFaces();
                var selectedFaceCount = selectedFaces.Length;
                
                updatedFaces.Clear();
                
                var totalLength = 0f;
                for (var fi = 0; fi < selectedFaceCount; fi++)
                {
                    if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(
                            "Processing faces",
                            "pb_Object: " + obj.name + " " + (fi + 1) + " / " + selectedFaceCount,
                            (i + (
                                (float)(fi + 1) / selectedFaceCount
                            )) / selected.Length
                        ))
                	{
                		UnityEditor.EditorUtility.ClearProgressBar();
                		Debug.LogWarning("User canceled Quad UV Strip processing.");
                		return new ActionResult(ActionResult.Status.Canceled, "User canceled Quad UV processing.");;
                	}

                    // Assume invalid for flatter flow control with continue
                    // down below
                    facesInvalid++;
                    
                    var face = selectedFaces[fi];

                    if (!face.IsQuad()) continue;
                    var indices = face.ToQuad();

                    if (selectedFaceCount < 2) continue;
                    
                    // Find the neighbor in front or behind
                    Face neighborFace = null;
                    var lastInStrip = false;
                    if (fi < selectedFaceCount - 1)
                    {
                        neighborFace = selectedFaces[fi + 1];
                    }
                    else if (fi > 0)
                    {
                        neighborFace = selectedFaces[fi - 1];
                        lastInStrip = true;
                    }

                    if (neighborFace == null) continue;
                    if (!neighborFace.IsQuad()) continue;
                    var neighbor = neighborFace.ToQuad();
                    var vertexNum = 4;

                    var faceIndexA = -1;
                    var faceIndexB = -1;
                    var neighborIndexA = -1;
                    var neighborIndexB = -1;
                    // Find the two vertices shared with neighbor
                    for (var vi = 0; vi < vertexNum; vi++)
                    {
                        var cv = vertices[indices[vi]];
                        for (var vj = 0; vj < vertexNum; vj++) {
                            var nv = vertices[neighbor[vj]];
                            if (cv == nv)
                            {
                                if (faceIndexA == -1)
                                {
                                    faceIndexA = vi;
                                    neighborIndexA = vj;
                                }
                                else if (faceIndexB == -1)
                                {
                                    faceIndexB = vi;
                                    neighborIndexB = vj;
                                }
                            }
                        }
                    }

                    if (faceIndexA == -1 || faceIndexB == -1) continue;
                    
                    var faceA = vertices[indices[faceIndexA]];
                    var faceB = vertices[indices[faceIndexB]];

                    // Set the other two vertices to be the ones left after the
                    // first two in the quad
                    var faceIndexC = (faceIndexA + 1) % 4;
                    if (faceIndexC == faceIndexB) faceIndexC = (faceIndexC + 1) % 4;
                    var faceIndexD = faceIndexC;
                    for (var di = 0; di < vertexNum; di++) {
                        faceIndexD = (faceIndexD + 1) % 4;
                        if (faceIndexD == faceIndexA) continue;
                        if (faceIndexD == faceIndexB) continue;
                        break;
                    }

                    var faceAB = faceB - faceA;
                    var faceAC = vertices[indices[faceIndexC]] - faceA;
                    var faceAD = vertices[indices[faceIndexD]] - faceA;
                    
                    // Flip the opposite vertices if ABCD don't form a U loop.
                    // This prevents butterflies in UV coordinates. In a U loop
                    // scenario, the AB∠AC angle is always smaller than AB∠AD.
                    // On the contrary, in crossed butterfly vertices it's
                    // always bigger. Dot product is the inverse as it's the
                    // cosine of the angle, so this is why it's less than here.
                    if (Vector3.Dot(faceAB, faceAC) < Vector3.Dot(faceAB, faceAD))
                    {
                        var c = faceIndexC;
                        faceIndexC = faceIndexD;
                        faceIndexD = c;
                    }

                    var faceC = vertices[indices[faceIndexC]];
                    var faceD = vertices[indices[faceIndexD]];

                    // Get the approximate length of the quad/face in the strip
                    // that is used for the UV
                    var faceSharedMid = (faceA + faceB) / 2f;
                    var faceOppositeMid = (faceC + faceD) / 2f;
                    var faceLength = Vector3.Distance(faceOppositeMid, faceSharedMid);

                    // Flip the coordinates for the last face in the strip as
                    // it refers to the previous neighbor, not the next one
                    if (lastInStrip)
                    {
                        var c = faceIndexC; var d = faceIndexD;
                        faceIndexC = faceIndexA;
                        faceIndexD = faceIndexB;
                        faceIndexA = c;
                        faceIndexB = d;
                    }
                    
                    face.manualUV = true;
                    uv[indices[faceIndexC]] = new Vector2(totalLength, 0);
                    uv[indices[faceIndexD]] = new Vector2(totalLength, 1);
                    totalLength += faceLength;
                    uv[indices[faceIndexA]] = new Vector2(totalLength, 1);
                    uv[indices[faceIndexB]] = new Vector2(totalLength, 0);
                    updatedFaces.Add(face);

                    // At this point the face was placed successfully, so fix
                    // the counts
                    facesInvalid--;
                    facesValid++;
                }
                
                obj.SetUVs(0, uv);
                obj.ToMesh();
                obj.Refresh();
            }

            // Rebuild the pb_Editor caches
            UnityEditor.ProBuilder.ProBuilderEditor.Refresh();

            Debug.Log("Quad UV Strip Finished\nMeshes: " + selected.Length + "  Faces: " + facesValid + "  Skipped Faces: " + facesInvalid);

            UnityEditor.EditorUtility.ClearProgressBar();
            return new ActionResult(ActionResult.Status.Success, "Quad UV Strip Finished\nMeshes: " + selected.Length + "  Faces: " + facesValid + "  Skipped Faces: " + facesInvalid);
        }
    }
}
