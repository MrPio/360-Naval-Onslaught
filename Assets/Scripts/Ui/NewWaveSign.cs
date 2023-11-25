using UnityEngine;


    public class NewWaveSign : MonoBehaviour
    {
        [SerializeField] private GameObject baseMain,baseHealthSlider;
        [SerializeField] private AudioClip menuIn,menuOut;
        public void MiddleAnimation()
        {
            baseMain.SetActive(true);
            baseHealthSlider.SetActive(true);
        }

        public void MenuIn() => MainCamera.AudioSource.PlayOneShot(menuIn);
        public void MenuOut() => MainCamera.AudioSource.PlayOneShot(menuOut);
    }

