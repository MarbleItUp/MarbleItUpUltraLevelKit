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
    sealed class QuadUVUnit : MenuAction
    {
        public override ToolbarGroup group
        {
            get { return ToolbarGroup.Selection; }
        }

        public override Texture2D icon
        {
            get { return IconUtility.GetIcon("Toolbar/Mode_Face"); }
        }

        public override TooltipContent tooltip
        {
            get { return s_Tooltip; }
        }

        static readonly TooltipContent s_Tooltip = new TooltipContent
            (
                "Quad UV Unit",
                "Moves the UVs of all selected quad faces to the unit square."
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
            Undo.RecordObjects(selected, "Quad UV");
            var facesValid = 0;
            var facesInvalid = 0;
            var updatedFaces = new List<Face>();
            var uv = new List<Vector4>();
            for(int i = 0; i < selected.Length; i++)
            {
                var obj = selected[i];
                obj.GetUVs(0, uv);
                var faces = obj.faces;
                var selectedFaceIndexes = obj.selectedFaceIndexes;
                var selectedFaceCount = obj.selectedFaceCount;

                updatedFaces.Clear();

                int processed = 0;
                foreach (int selectedFaceIndex in selectedFaceIndexes)
                {
                    if (UnityEditor.EditorUtility.DisplayCancelableProgressBar(
                            "Processing faces",
                            "pb_Object: " + obj.name + " " + (processed + 1) + " / " + selectedFaceCount,
                            (i + (
                                (float)(processed + 1) / selectedFaceCount
                            )) / selected.Length
                        ))
                    {
                        UnityEditor.EditorUtility.ClearProgressBar();
                        Debug.LogWarning("User canceled Quad UV processing.");
                        return new ActionResult(ActionResult.Status.Canceled, "User canceled Quad UV processing.");
                    }

                    var face = faces[selectedFaceIndex];
                    
                    if (face.IsQuad())
                    {
                        var indices = face.ToQuad();
                        facesValid++;
                        face.manualUV = true;
                        uv[indices[0]] = new Vector2(0, 0);
                        uv[indices[1]] = new Vector2(0, 1);
                        uv[indices[2]] = new Vector2(1, 1);
                        uv[indices[3]] = new Vector2(1, 0);
                        updatedFaces.Add(face);
                    }
                    else
                    {
                        facesInvalid++;
                    }

                    processed++;
                }
                
                obj.SetUVs(0, uv);
                obj.ToMesh();
                obj.Refresh();
            }

            UnityEditor.ProBuilder.ProBuilderEditor.Refresh();

            Debug.Log("Quad UV Finished\nMeshes: " + selected.Length + "  Faces: " + facesValid + "  Skipped Faces: " + facesInvalid);

            UnityEditor.EditorUtility.ClearProgressBar();
            return new ActionResult(ActionResult.Status.Success, "Quad UV Finished\nMeshes: " + selected.Length + "  Faces: " + facesValid + "  Skipped Faces: " + facesInvalid);
        }
    }
}
