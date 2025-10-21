using UnityEngine;
using Zenject;
using Unity.Cinemachine;

//[CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
public class PlayerInstaller : MonoInstaller<PlayerInstaller>
{
    [SerializeField] private CinemachineCamera FpCamera;
    [SerializeField] private CharacterController characterController;

    public override void InstallBindings()
    {
        Container.Bind<CharacterController>()
                 .FromComponentInHierarchy()
                 .AsSingle()
                 .NonLazy();
         AutoBindCinemachineCameras();
        
        //PlayerBindings();
    }

    private void PlayerBindings()
    {
       
    }


    private void AutoBindCinemachineCameras()
    {
        var allCams = GameObject.FindObjectsByType<CinemachineCamera>(
            FindObjectsInactive.Include, 
            FindObjectsSortMode.None);

        if (allCams == null || allCams.Length == 0)
        {
            Debug.LogWarning("No CinemachineCamera found in the scene to bind.");
            return;
        }

        foreach (var cam in allCams)
        {
            Debug.Log($"[CameraAutoInstaller] Bind camera: {cam.name}");

            Container.BindInstance(cam)
                     .WithId(cam.name)
                     .AsSingle();
        }
    }
}