using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;

namespace MIU.ProBuilder
{
    [ProBuilderMenuAction]
    sealed class SnapUVToGrid : MenuAction
    {
        static Pref<float> s_GridSnapIncrement = new Pref<float>("uv.uvEditorGridSnapIncrement", .125f, SettingsScope.Project);
        
        public override ToolbarGroup group
        {
            get { return ToolbarGroup.Selection; }
        }

        public override Texture2D icon
        {
            get { return IconUtility.GetIcon("Toolbar/Face_Subdivide"); }
        }

        public override TooltipContent tooltip
        {
            get { return s_Tooltip; }
        }

        static readonly TooltipContent s_Tooltip = new TooltipContent
            (
                "Snap UV to Grid",
                "Snap each UV coordinate to the grid."
            );

        public override SelectMode validSelectModes
        {
            get { return SelectMode.TextureVertex | SelectMode.Vertex; }
        }

        public override bool enabled
        {
            get { return base.enabled && MeshSelection.selectedVertexCount > 0; }
        }

        public override ActionResult DoAction()
        {
            IEnumerable<ProBuilderMesh> meshes;
            
            meshes = MeshSelection.top;
            
            UndoUtility.RecordSelection("Snap UV Coordinates to Grid");

            var uvs = new List<Vector4>();

            foreach (var mesh in MeshSelection.top) {
                uvs.Clear();
                mesh.GetUVs(0, uvs);
                var selectedVertexIndices = mesh.selectedVertices;

                foreach (var vertexIndex in mesh.selectedVertices) {
                    var uv = uvs[vertexIndex];
                    uv.x = ProGridsSnapping.SnapValue(uv.x, s_GridSnapIncrement);
                    uv.y = ProGridsSnapping.SnapValue(uv.y, s_GridSnapIncrement);
                    uv.z = ProGridsSnapping.SnapValue(uv.z, s_GridSnapIncrement);
                    uv.w = ProGridsSnapping.SnapValue(uv.w, s_GridSnapIncrement);
                    uvs[vertexIndex] = uv;
                }

                mesh.SetUVs(0, uvs);
            }

            ProBuilderEditor.Refresh();

            return new ActionResult(ActionResult.Status.Success, "Snap UV Coordinates to Grid");
        }
    }
}
