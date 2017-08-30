using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum MessageType
{
    Info,
    Error,
    OK,
}

//public enum ConfirmationMessageAction
//{
//    QuitApplication,
//    QuitGame,
//    QuitEditing,
//    SavingWorld,
//}

public class Message {

    //public static string[] MessagesList =
    //{
    //    "Are you sure want to leave the application?",//0
    //    "Will you leave the Map Editor?",//1
    //    "Will you leave the game?"//2
    //};

    #region MessageText Methods

    /// <summary>
    /// Shows a text on screen
    /// </summary>
    /// <param name="text">The message</param>
    /// <param name="time">The time in seconds of the message to be destroy</param>
    public static void showMessageText(string text, int time = 2)
    {
        new MessageText(text, time);
    }

    /// <summary>
    /// Shows a text on screen
    /// </summary>
    /// <param name="text">"The message</param>
    /// <param name="type">The type of message (Error, Info ...)</param>
    /// <param name="time">The time in seconds of the message to be destroy</param>
    public static void showMessageText(string text,MessageType type, int time = 2)
    {
        new MessageText(text, type, time);
    }

    #endregion

    //#region ConfirmationMessage Methods

    //public static void showConfirmationMessage(string theMessage, ConfirmationMessageAction theActionToPerform)
    //{
    //    new ConfirmationMessage(theMessage, theActionToPerform);
    //}

    //public static void showConfirmationMessage(string theMessage, ConfirmationMessageAction theActionToPerform, string yesButtonText, string noButtonText)
    //{
    //    new ConfirmationMessage(theMessage, theActionToPerform, yesButtonText, noButtonText);
    //}

    //public static void showConfirmationMessage(int messageIndex, ConfirmationMessageAction theActionToPerform)
    //{
    //    new ConfirmationMessage(MessagesList[messageIndex], theActionToPerform);
    //}

    //public static void showConfirmationMessage(int messageIndex, ConfirmationMessageAction thePerformingAction, string yesButtonText, string noButtonText)
    //{
    //    new ConfirmationMessage(MessagesList[messageIndex], thePerformingAction, yesButtonText, noButtonText);
    //}

    //#endregion

    private class MessageText
    {
        GameObject theGameObject = Resources.Load("Messages/MessageTextPrefab")as GameObject;

        public MessageText(string text, int timeToDestroy)
        {
            GameObject mt = MonoBehaviour.Instantiate(theGameObject);
            mt.GetComponentInChildren<Text>().text = text;
            MonoBehaviour.Destroy(mt, timeToDestroy);
        }

        public MessageText(string text, MessageType type, int timeToDestroy)
        {
            GameObject mt = MonoBehaviour.Instantiate(theGameObject);
            if (type.Equals(MessageType.Error))
            {
                mt.GetComponentInChildren<Text>().color = Color.red;
            }
            else
                if (type.Equals(MessageType.OK))
            {
                mt.GetComponentInChildren<Text>().color = Color.green;
            }
            else
            {
                mt.GetComponentInChildren<Text>().color = Color.black;
            }
            mt.GetComponentInChildren<Text>().text = text;
            MonoBehaviour.Destroy(mt, timeToDestroy);
        }
    }

    //private class ConfirmationMessage
    //{
    //    private GameObject theGameObject = Resources.Load("Messages/ConfirmationMessagePrefab") as GameObject;//The confirmation mwnu prefab

    //    public ConfirmationMessage(string theMessage, ConfirmationMessageAction actionToPerform)
    //    {            
    //        GameObject g= MonoBehaviour.Instantiate(theGameObject);
    //        g.GetComponent<confirmationMessageScript>().PerformingAction = actionToPerform;
    //        g.GetComponent<confirmationMessageScript>().TheMessage = theMessage;
    //    }

    //    public ConfirmationMessage(string theMessage, ConfirmationMessageAction actionToPerform, string yesButtonText, string noButtonText)
    //    {
    //        GameObject g = MonoBehaviour.Instantiate(theGameObject);
    //        g.GetComponent<confirmationMessageScript>().PerformingAction = actionToPerform;
    //        g.GetComponent<confirmationMessageScript>().TheMessage = theMessage;
    //        g.GetComponent<confirmationMessageScript>().YesButtonText = yesButtonText;
    //        g.GetComponent<confirmationMessageScript>().NoButtonText = noButtonText;
    //    }
    //}
}
