using System;
using System.Collections.Generic;

using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AmazonSQS_Sample
{
    class Program
    {
        public static void Main(string[] args)
        {
            IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.USEast2);

            try
            {
                insert_separator("App Console using Amazon SQS");

                bool appContinue = true;
                string myQueueUrl = "";
                string myMessage = "";

                do
                {

                    // new test comment
                    insert_message("Insert Option Number");
                    insert_message(" 1. Create SQS Queue");
                    insert_message(" 2. Show list SQS Queue");
                    insert_message(" 3. Send SQS Message");
                    insert_message(" 4. Get SQS Message");
                    insert_message(" 5. Remove SQS Message");
                    insert_message(" -- ");
                    insert_message(" 7. Exit");
                    insert_message("");
                    insert_message("Note: To select a SQS Queue you need to insert option 1 and insert the Queue name");
                    
                    int option = 0;
                    int.TryParse(Console.ReadLine(), out option);

                    Console.WriteLine("\n");

                    switch(option)
                    {
                        case 0:
                            //No input inserted, display again the menu
                            break;
                        case 1:
                            //Creating a queue
                            create_sqs_queue(sqs, out myQueueUrl);
                            break;
                        case 2:
                            //List of Amazon SQS
                            show_sqs_queue_list(sqs);
                            break;
                        case 3:
                            //Send SQS Message
                            send_sqs_message(sqs, myQueueUrl);
                            break;
                        case 4:
                            //Receiving SQS Message
                            get_sqs_message(sqs, myQueueUrl, out myMessage);
                            break;
                        case 5:
                            //Remove SQS Message
                            remove_sqs_message(sqs, myQueueUrl, myMessage);
                            break;
                        case 7:
                            appContinue = false;
                            break;
                    }                  
                } while (appContinue);
                
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
            }
            
        }

        public static void insert_separator(string message)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine(message);
            Console.WriteLine("===========================================\n");
        }

        public static void insert_message(string message)
        {
            Console.WriteLine("**** " + message + " ****\n");
        }

        public static void create_sqs_queue(IAmazonSQS sqs, out string myQueueUrl)
        {
            insert_message("Insert SQS Queue Name");
            var name = Console.ReadLine();
            Console.WriteLine();

            CreateQueueRequest sqsRequest = new CreateQueueRequest();
            sqsRequest.QueueName = name;

            CreateQueueResponse createQueueResponse = sqs.CreateQueue(sqsRequest);
            myQueueUrl = createQueueResponse.QueueUrl;
            insert_message("[" + name + "] queue was created correctly.");
        }

        public static void show_sqs_queue_list(IAmazonSQS sqs)
        {
            ListQueuesRequest listQueuesRequest = new ListQueuesRequest();
            ListQueuesResponse listQueuesResponse = sqs.ListQueues(listQueuesRequest);

            Console.WriteLine("Printing list of Amazon SQS queues.\n");
            foreach (String queueUrl in listQueuesResponse.QueueUrls)
            {
                Console.WriteLine("  QueueUrl: {0}", queueUrl);
            }
            Console.WriteLine();
        }
        
        public static void send_sqs_message(IAmazonSQS sqs, string myQueueUrl)
        {
            if(!string.IsNullOrEmpty(myQueueUrl))
            {
                Console.WriteLine("Insert Message");
                var message = Console.ReadLine();
                Console.WriteLine();

                SendMessageRequest sendMessageRequest = new SendMessageRequest();
                sendMessageRequest.QueueUrl = myQueueUrl;
                sendMessageRequest.MessageBody = message;
                sqs.SendMessage(sendMessageRequest);

                insert_message("Message added correctly.");
            }
            else
            {
                insert_message("Invalid Queue Name");
            }
        }

        public static void get_sqs_message(IAmazonSQS sqs, string myQueueUrl, out string myMessage)
        {

            if(!string.IsNullOrEmpty(myQueueUrl))
            {
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
                receiveMessageRequest.QueueUrl = myQueueUrl;
                receiveMessageRequest.MaxNumberOfMessages = 5;

                ReceiveMessageResponse receiveMessageResponse = sqs.ReceiveMessage(receiveMessageRequest);

                if (receiveMessageResponse.Messages.Count > 0)
                    myMessage = receiveMessageResponse.Messages[0]?.ReceiptHandle ?? "";
                else
                    myMessage = string.Empty;

                Console.WriteLine("Printing received message.\n");
                foreach (Message message in receiveMessageResponse.Messages)
                {
                    Console.WriteLine("  Message");
                    Console.WriteLine("    MessageId: {0}", message.MessageId);
                    Console.WriteLine("    ReceiptHandle: {0}", message.ReceiptHandle);
                    Console.WriteLine("    MD5OfBody: {0}", message.MD5OfBody);
                    Console.WriteLine("    Body: {0}", message.Body);

                    foreach (KeyValuePair<string, string> entry in message.Attributes)
                    {
                        Console.WriteLine("  Attribute");
                        Console.WriteLine("    Name: {0}", entry.Key);
                        Console.WriteLine("    Value: {0}", entry.Value);
                    }
                }
            }
            else
            {
                myMessage = string.Empty;
                insert_message(" Invalid Queue Name");
            }
            
        }

        public static void remove_sqs_message(IAmazonSQS sqs, string myQueueUrl, string myMessage)
        {
            if(!string.IsNullOrEmpty(myQueueUrl) && !string.IsNullOrEmpty(myMessage))
            {
                Console.WriteLine("Deleting the message.\n");
                DeleteMessageRequest deleteRequest = new DeleteMessageRequest();
                deleteRequest.QueueUrl = myQueueUrl;
                deleteRequest.ReceiptHandle = myMessage;
                sqs.DeleteMessage(deleteRequest);
            }
            else
            {
                insert_message("Invalid Queue/Message");
            }
        }

    }
}