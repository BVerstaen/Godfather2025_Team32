using System.Collections;
using UnityEngine;

public class ControllerActivator : MonoBehaviour
{
    [SerializeField] private GameObject _obj;
    private void OnEnable()
    {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
        _obj.SetActive(true);
    }
}
