using UnityEngine;

public class ARInteractionManager : MonoBehaviour
{
    public void OnStudentSelected(GameObject studentObject)
    {
        Student studentData = studentObject.GetComponent<Student>(); // Your student component
        ARNotebookManager.Instance.AddItemToNotebook(
            studentData.studentName,
            "student",
            studentData.additionalInfo,
            studentObject.transform.position
        );
    }
    
    public void OnObjectSelected(GameObject arObject)
    {
        ARNotebookManager.Instance.AddItemToNotebook(
            arObject.name,
            "object",
            "Custom object description",
            arObject.transform.position
        );
    }
    
    public void OnRoomSelected(GameObject roomObject)
    {
        ARNotebookManager.Instance.AddItemToNotebook(
            roomObject.name,
            "room",
            "Room description",
            roomObject.transform.position
        );
    }
}
