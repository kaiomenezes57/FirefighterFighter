using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using VivoxUnity;

namespace FirefighterFighter.Networking
{
    public class ChatManager : MonoBehaviour
    {
        public ILoginSession LoginSession;
        public IChannelSession channelSession;

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            VivoxService.Instance.Initialize();
        }

        public void JoinChannel(string channelName, ChannelType channelType, bool connectAudio, bool connectText, bool transmissionSwitch = true, Channel3DProperties properties = null)
        {
            if (LoginSession.State == LoginState.LoggedIn)
            {
                Channel channel = new(channelName, channelType, properties);
                channelSession = LoginSession.GetChannelSession(channel);

                channelSession.BeginConnect(connectAudio, connectText, transmissionSwitch, channelSession.GetConnectToken(), ar =>
                {
                    try
                    {
                        channelSession.EndConnect(ar);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Could not connect to channel: {e.Message}");
                        return;
                    }
                });
            }
            else
            {
                Debug.LogError("Can't join a channel when not logged in.");
            }
        }

        // For this example, _channelSession is a connected IChannelSession and _accountId is the local user’s AccountId.
        // Sending
        [ContextMenu("Test")]
        private void SendGroupMessage()
        {
            Channel channel = new("Chat", type: ChannelType.NonPositional, properties: null);
            //channelSession = LoginSession.GetChannelSession(channel);

            string channelName = "Chat";
            string senderName = "Kaio";
            string message = "Hello World!";

            channelSession.BeginSendText(message, ar =>
            {
                try
                {
                    channelSession.EndSendText(ar);
                }
                catch (Exception e)
                {
                    // Handle error
                    return;
                }
                Debug.Log(channelName + ": " + senderName + ": " + message);
            });

            // Receiving
            channelSession.MessageLog.AfterItemAdded += OnChannelMessageReceived;

        }

        private void OnChannelMessageReceived(object sender, QueueItemAddedEventArgs<IChannelTextMessage> queueItemAddedEventArgs)
        {
            string channelName = queueItemAddedEventArgs.Value.ChannelSession.Channel.Name;
            string senderName = queueItemAddedEventArgs.Value.Sender.Name;
            string message = queueItemAddedEventArgs.Value.Message;

            Debug.Log(channelName + ": " + senderName + ": " + message);
        }
    }
}