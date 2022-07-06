using UnityEngine;

namespace MapChanger.Map
{
    internal class QoL : HookModule
    {
        public override void Hook()
        {
            On.GameMap.Update += ZoomFasterOnKeyboard;
            On.GameManager.UpdateGameMap += DisableUpdatedMapPrompt;
        }

        public override void Unhook()
        {
            On.GameMap.Update -= ZoomFasterOnKeyboard;
            On.GameManager.UpdateGameMap -= DisableUpdatedMapPrompt;
        }

        private static void ZoomFasterOnKeyboard(On.GameMap.orig_Update orig, GameMap self)
        {
            if (Settings.MapModEnabled
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
            if (Settings.MapModEnabled)
            {
                return false;
            }

            return orig(self);
        }
    }
}
