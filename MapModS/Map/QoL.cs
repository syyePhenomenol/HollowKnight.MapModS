using UnityEngine;

namespace MapModS.Map
{
    internal class QoL : HookModule
    {
        internal override void Hook()
        {
            On.GameMap.Update += ZoomFasterOnKeyboard;
            On.GameManager.UpdateGameMap += DisableUpdatedMapPrompt;
        }

        internal override void Unhook()
        {
            On.GameMap.Update -= ZoomFasterOnKeyboard;
        }

        private static void ZoomFasterOnKeyboard(On.GameMap.orig_Update orig, GameMap self)
        {
            if (MapModS.LS.ModEnabled
                && self.canPan
                && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (InputHandler.Instance.inputActions.down.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y + self.panSpeed * Time.deltaTime, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.up.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y - self.panSpeed * Time.deltaTime, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.left.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x + self.panSpeed * Time.deltaTime, self.transform.position.y, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.right.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x - self.panSpeed * Time.deltaTime, self.transform.position.y, self.transform.position.z);
                }
            }

            orig(self);
        }

        private static bool DisableUpdatedMapPrompt(On.GameManager.orig_UpdateGameMap orig, GameManager self)
        {
            orig(self);

            return false;
        }
    }
}
