using UnityEngine;
using UnityEngine.UIElements;

namespace STAGE.UI
{
    public class DragManipulator: IManipulator
    {
        public VisualElement _target;
        public Vector3 offset;
        public PickingMode Mode;
        public bool Dragging = false;
        public VisualElement target
        {
            get => _target;
            set
            {
                _target = value;

                _target.RegisterCallback<PointerDownEvent>(DragBegin);
                _target.RegisterCallback<PointerUpEvent>(DragEnd);
                _target.RegisterCallback<PointerMoveEvent>(PointerMove);
            }
        }

        public void DragBegin(PointerDownEvent evt)
        {
            Mode = target.pickingMode;
            target.pickingMode = PickingMode.Ignore;
            offset = evt.localPosition;
            Dragging = true;
            target.CapturePointer(evt.pointerId);
        }

        public void DragEnd(IPointerEvent evt)
        {
            target.ReleasePointer(evt.pointerId);
            Dragging = false;
            target.pickingMode = Mode;
        }
        public void PointerMove(PointerMoveEvent evt)
        {
            if (Dragging)
            {
                Vector3 delta = evt.localPosition - (Vector3)offset;
                target.transform.position += delta;
            }
        }
    }
}
