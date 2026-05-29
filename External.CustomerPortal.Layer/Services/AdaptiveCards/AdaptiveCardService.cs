using AdaptiveCards;
using AdaptiveCards.Templating;
using BotDialog.Layer.Dialogs;
using Common.Layer.Models;
using Common.Layer.Models.AdaptiveCard;
using Common.Layer.Models.AppSettings;
using Common.Layer.Models.WebChatBot;
using External.CustomerPortal.Layer.ExceptionLog;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace External.CustomerPortal.Layer.Services.AdaptiveCards
{
    public class AdaptiveCardService : IAdaptiveCardService
    {
        private const string _welcomeCardCacheKey = "_welcome-card";
        private readonly AppSettingsModel _appSettings;

        /// <summary>
        /// Memory cache instance to store and retrieve adaptive card payload.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        private readonly int CardCacheInHours = 12;

        public AdaptiveCardService(IMemoryCache memoryCache, IOptions<AppSettingsModel> appSettings)
        {
            this.memoryCache = memoryCache;
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        #region PRIVATE METHODS

        private string GetCardPayload(string cardCacheKey, string jsonTemplateFileName)
        {
            //bool isCacheEntryExists = memoryCache.TryGetValue(cardCacheKey, out string cardPayload);

            //if (!isCacheEntryExists)
            //{
            //    var mainDirectoryPath = Directory.GetCurrentDirectory() + @"\Documents_Internal";

            //    // If cache duration is not specified then by default cache for 12 hours.
            //    var cacheDurationInHour = TimeSpan.FromHours(CardCacheInHours);
            //    cacheDurationInHour = cacheDurationInHour.Hours <= 0 ? TimeSpan.FromHours(12) : cacheDurationInHour;

            //    var cardJsonFilePath = Path.Combine(mainDirectoryPath, $".\\AdaptiveCards\\{jsonTemplateFileName}");
            //    cardPayload = File.ReadAllText(cardJsonFilePath);
            //    memoryCache.Set(cardCacheKey, cardPayload, cacheDurationInHour);
            //}

            // wwwroot/Templates/AdaptiveCards folder
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "AdaptiveCards");
            var cardJsonFilePath = Path.Combine(rootPath, jsonTemplateFileName);

            if (!File.Exists(cardJsonFilePath))
                throw new FileNotFoundException($"Template file '{jsonTemplateFileName}' not found at path: {cardJsonFilePath}");

            var cardPayload = File.ReadAllText(cardJsonFilePath);
            return cardPayload;
        }

        #endregion

        public Attachment GetCard_WelcomeMessage_PersonalScope(WelcomeCardModel data)
        {
            var cardPayload = GetCardPayload(_welcomeCardCacheKey, "\\welcomeCard.json");
            var template = new AdaptiveCardTemplate(cardPayload);

            var cardJson = template.Expand(data);
            AdaptiveCard card = AdaptiveCard.FromJson(cardJson).Card;

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };

            return adaptiveCardAttachment;
        }

        public Attachment CreateCard_WelcomeMessage_PersonalScope(WelcomeCardModel data)
        {
            // Create the container for ShortDesc
            var shortDescContainer = new
            {
                type = "Container",
                width = "stretch",
                items = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = data.ShortDesc,
                        weight = "bolder",
                        size = "large",
                        horizontalAlignment = "left",
                        wrap = true
                    }
                }
            };

            // Create the container for the Image
            var imageContainer = new
            {
                type = "Container",
                width = "stretch",
                items = new List<object>
                {
                    new
                    {
                        type = "Image",
                        url = data.ImageUrl,
                        altText = "SOP Search Expert",
                        horizontalAlignment = "left",
                        spacing = "Medium"
                    }
                }
            };

            // Create the container for LongDesc
            var longDescContainer = new
            {
                type = "Container",
                width = "stretch",
                items = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = data.LongDesc,
                        wrap = true,
                        spacing = "Medium",
                        size = "Medium"
                    }
                }
            };

            // Combine all containers into the card body
            var cardContent = new
            {
                schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                type = "AdaptiveCard",
                version = "1.4",
                body = new List<object>
                {
                    shortDescContainer,
                    imageContainer,
                    longDescContainer
                }
            };

            // Serialize card content to JSON
            string cardJson = JsonConvert.SerializeObject(cardContent);

            // Create and return the Attachment
            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardContent
            };
        }

        public Attachment CreateCard_LikeDislike_PersonalScope_InRows(WebChatUserMessageModel data)
        {
            try
            {
                var actionButtons = new List<object>
                {
                    new { type = "Column", width = "250px", items = new List<object> { new { type = "ActionSet", actions = new List<object> { new { type = "Action.Submit", title = "👍", tooltip = "Like", data = new { action = "feedback", webChatLogId = data.WebChatLogId, messageId = data.MessageActivityId, feedback = "Like" } } } } } },
                };

                var cardContent = new
                {
                    type = "AdaptiveCard",
                    version = "1.5",
                    schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                    body = new List<object>
                    {
                        new { type = "TextBlock", text = "Was this response helpful?", weight = "Bolder", size = "Medium", wrap = true, horizontalAlignment = "Center", spacing = "Small" },
                        new { type = "Container", items = new List<object> { new { type = "ColumnSet", columns = actionButtons, horizontalAlignment = "Center" } } }
                    }
                };

                string cardJson = JsonConvert.SerializeObject(cardContent);
                Console.WriteLine(cardJson);

                return new Attachment { ContentType = "application/vnd.microsoft.card.adaptive", Content = cardContent };
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public Attachment CreateCard_LikeDislike_PersonalScope(WebChatUserMessageModel data)
        {
            try
            {
                var cardContent = new
                {
                    type = "AdaptiveCard",
                    version = "1.5",
                    schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                    body = new List<object>
                    {
                        new
    {
        type = "TextBlock",
        text = "Was this response helpful?",
        weight = "Bolder",
        size = "Medium",
        wrap = true
    },
    new
    {
        type = "Input.Text",
        id = "feedbackComment",
        placeholder = "Add your comments here...",
        isMultiline = true
    },
                        new
                        {
                            type = "ColumnSet",
                            horizontalAlignment = "Left",
                            columns = new List<object>
                            {
                                //new
                                //{
                                //    type = "Column",
                                //    width = "auto",
                                //    verticalContentAlignment = "Center",
                                //    items = new List<object>
                                //    {
                                //        new
                                //        {
                                //            type = "TextBlock",
                                //            text = "Was this response helpful?",
                                //            weight = "Bolder",
                                //            size = "Medium",
                                //            wrap = true
                                //        }
                                //    }
                                //},
                                new
                                {
                                    type = "Column",
                                    width = "auto",
                                    verticalContentAlignment = "Center",
                                    items = new List<object>
                                    {
                                        new
                                        {
                                            type = "ActionSet",
                                            actions = new List<object>
                                            {
                                                new
                                                {
                                                    type = "Action.Submit",
                                                    title = "👍 Yes",
                                                    tooltip = "Like",
                                                    style = "default",
                                                    data = new
                                                    {
                                                        action = "feedback",
                                                        webChatLogId = data.WebChatLogId,
                                                        messageId = data.MessageActivityId,
                                                        feedback = "Like"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new
                                {
                                    type = "Column",
                                    width = "auto",
                                    verticalContentAlignment = "Center",
                                    items = new List<object>
                                    {
                                        new
                                        {
                                            type = "ActionSet",
                                            actions = new List<object>
                                            {
                                                new
                                                {
                                                    type = "Action.Submit",
                                                    title = "👎 No",
                                                    tooltip = "Dislike",
                                                    style = "default",
                                                    data = new
                                                    {
                                                        action = "feedback",
                                                        webChatLogId = data.WebChatLogId,
                                                        messageId = data.MessageActivityId,
                                                        feedback = "Dislike"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                string cardJson = JsonConvert.SerializeObject(cardContent);
                Console.WriteLine(cardJson);

                return new Attachment { ContentType = "application/vnd.microsoft.card.adaptive", Content = cardContent };
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        //public Attachment CreateCard_LikeDislike_Response_PersonalScope(string message, string feedback)
        //{
        //    try
        //    {
        //        var updatedAdaptiveCard = new
        //        {
        //            type = "AdaptiveCard",
        //            version = "1.5",
        //            body = new List<object>
        //            {
        //                new
        //                {
        //                    type = "TextBlock",
        //                    text = $"{message}",
        //                    weight = "Bolder",
        //                    size = "Medium",
        //                    wrap = true,
        //                    horizontalAlignment = "Left"
        //                }
        //            }
        //        };

        //        string cardJson = JsonConvert.SerializeObject(updatedAdaptiveCard);
        //        Console.WriteLine(cardJson);

        //        return new Attachment
        //        {
        //            ContentType = "application/vnd.microsoft.card.adaptive",
        //            Content = updatedAdaptiveCard
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //        return null;
        //    }
        //}

        //public Attachment CreateCard_LikeDislike_Response_PersonalScope(string message, string feedback)
        //{
        //    try
        //    {
        //        var updatedAdaptiveCard = new Dictionary<string, object>
        //        {
        //            ["type"] = "AdaptiveCard",
        //            ["version"] = "1.5",
        //            ["body"] = new List<object>
        //    {
        //        new
        //        {
        //            type = "TextBlock",
        //            text = message,
        //            weight = "Bolder",
        //            size = "Medium",
        //            wrap = true,
        //            horizontalAlignment = "Left"
        //        }
        //    }
        //        };

        //        // Conditionally add "Create Ticket" button if feedback is "dislike"
        //        if (feedback?.ToLower() == "dislike")
        //        {
        //            updatedAdaptiveCard["actions"] = new List<object>
        //    {
        //        new
        //        {
        //            type = "Action.OpenUrl",
        //            title = "Create Ticket",
        //            style = "positive",
        //            horizontalAlignment = "Left",
        //            url = Environment.GetEnvironmentVariable("CREATE_TICKET_URL") ??
        //                  "https://supporthub.embee.co.in/support/tickets/new"
        //        }
        //    };
        //        }

        //        string cardJson = JsonConvert.SerializeObject(updatedAdaptiveCard);
        //        Console.WriteLine(cardJson);

        //        return new Attachment
        //        {
        //            ContentType = "application/vnd.microsoft.card.adaptive",
        //            Content = updatedAdaptiveCard
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //        return null;
        //    }
        //}

        public Attachment CreateCard_LikeDislike_Response_PersonalScope(string message, string feedback)
        {
            try
            {
                // Card body
                var body = new List<object>
        {
            new
            {
                type = "TextBlock",
                text = message,
                weight = "Bolder",
                size = "Medium",
                wrap = true,
                horizontalAlignment = "Left"
            }
        };

                // If feedback is "dislike", add a Create Ticket button wrapped in a ColumnSet
                if (feedback?.ToLower() == "dislike")
                {
                    body.Add(new
                    {
                        type = "ColumnSet",
                        columns = new List<object>
                {
                    new
                    {
                        type = "Column",
                        width = "auto",
                        items = new List<object>
                        {
                            new
                            {
                                type = "ActionSet",
                                actions = new List<object>
                                {
                                    new
                                        {
                                            type = "Action.Submit",
                                            title = "Create Ticket",
                                            data = new
                                            {
                                                msteams = new { type = "task/fetch" },
                                                action = "createTicket",
                                                url = Environment.GetEnvironmentVariable("CREATE_TICKET_URL") ?? "https://supporthub.embee.co.in/support/tickets/new"
                                            },
                                            style = "positive"
                                        }
                                }
                            }
                        }
                    }
                }
                    });
                }

                var updatedAdaptiveCard = new Dictionary<string, object>
                {
                    ["type"] = "AdaptiveCard",
                    ["version"] = "1.5",
                    ["body"] = body
                };

                return new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = updatedAdaptiveCard
                };
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public Attachment CreateCard_WebChatOptions_PersonalScope(List<AdaptiveCardModel> optionList)
        {
            try
            {
                // Separate options (Id > 0) and actions (Id <= 0)
                var optionContainers = optionList
                    .Where(option => option.Id > 0)
                    .Select(option => new
                    {
                        type = "Container",
                        backgroundColor = "#E3F2FD",
                        items = new List<object>
                        {
                        new
                        {
                            type = "TextBlock",
                            text = option.Name,
                            wrap = true,
                            size = "Default",
                            weight = "Bolder",
                            color = "Default",
                            horizontalAlignment = "Left"
                        }
                        },
                        selectAction = new
                        {
                            type = "Action.Submit",
                            title = option.Name,
                            data = new { action = "optionSubmit", optionId = option.Id }
                        },
                        style = "accent"
                    }).ToList();

                // Process action sets (Id <= 0)
                //var actionButtons = optionList
                //    .Where(option => option.Id <= 0)
                //    .Select(action => new
                //    {
                //        type = "Column",
                //        width = "stretch",
                //        items = new List<object>
                //        {
                //            new
                //            {
                //                type = "ActionSet",
                //                actions = new List<object>
                //                {
                //                    new
                //                    {
                //                        type = "Action.Submit",
                //                        title = action.Name,
                //                        data = new { action = action.Id == 0 ? "endChat" : action.Id == -3 ? "others" : "goBack" },
                //                        style = action.Id == 0 ? "destructive" : action.Id == -3 ? "positive" : null
                //                    }
                //                }
                //            }
                //        }
                //    }).ToList();

                // Create the Adaptive Card content
                var cardContent = new
                {
                    type = "AdaptiveCard",
                    version = "1.3",
                    body = new List<object>
                    {
                        new
                        {
                            type = "TextBlock",
                            text = "Are you looking for below supports? If not, enter your query.",
                            weight = "Bolder",
                            size = "Medium",
                            horizontalAlignment = "Left",
                            wrap = true,
                            color = "Accent",
                            fontType = "Default"
                        }
                    }
                    .Concat(optionContainers).ToList(),
                    //.Append(new  // Action Buttons in a ColumnSet
                    //{
                    //    type = "Container",
                    //    items = new List<object>
                    //    {
                    //        new
                    //        {
                    //            type = "ColumnSet",
                    //            columns = actionButtons,
                    //            horizontalAlignment = "Center"
                    //        }
                    //    },
                    //    horizontalAlignment = "Center",
                    //    style = "emphasis"
                    //}).ToList(),
                    schema = "http://adaptivecards.io/schemas/adaptive-card.json"
                };

                // Serialize the card content for debugging
                string cardJson = JsonConvert.SerializeObject(cardContent);
                Console.WriteLine(cardJson);

                return new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = cardContent
                };
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public Attachment? CreateCard_UserMessage_PersonalScope(WebChatUserMessageModel data)
        {
            try
            {
                string greetingMsg = "! \n I'm EIS Automation BOT, professional tool tailored for efficient SOP retrieval. It leverages advanced search algorithms to deliver accurate results, making it an indispensable resource for professionals seeking self-help through SOPs.";
                string apologyMsg = "\n Apologies, I could not find the information you’re looking for. Could you please provide more details or clarify your query? \n Here are a few tips to help refine your search: \n • **Be Specific**: Include keywords or phrases related to the document or topic you need \n • **Check Spelling**: Ensure there are no typos or errors in your query \n • **Context Matters**: Providing context or additional information can help narrow down the search results.";

                string[] replyLines;

                string sentence = data.Response.ToLower();

                string[] phrasesToFind = { "sorry", "out of scope", "The requested information is not available in the retrieved data", "the retrieved documents do not contain any information" };

                // Check if any phrase is present in the sentence
                bool containsWord = phrasesToFind.Any(phrase => sentence.Contains(phrase, StringComparison.OrdinalIgnoreCase));

                // Determine reply lines
                //if (data.FileList != null && data.FileList.Any() && !containsWord)
                if (!containsWord)
                {
                    replyLines = data.Response.Split('\n');
                }
                else
                {
                    string message = "Hi " + data.Name + greetingMsg;

                    if (data.Response != null && data.Response.TrimStart().StartsWith("hello!", StringComparison.OrdinalIgnoreCase))
                    {
                        replyLines = message.Split('\n');
                    }
                    else
                    {
                        replyLines = (containsWord ? message + " " + apologyMsg : message).Split('\n');
                    }
                }


                //                // Create the container for reply lines
                //                var replyLinesContainer = new
                //                {
                //                    type = "Container",
                //                    items = new List<object>
                //    {
                //        new
                //        {
                //            type = "Container",
                //            style = "emphasis",
                //            spacing = "Large",
                //            items = replyLines.Select(line => new
                //            {
                //                type = "TextBlock",
                //                text = line,
                //                wrap = true,
                //                size = "Medium",
                //                color = "Default",
                //                spacing = "Small",
                //                horizontalAlignment = "Left"
                //            }).ToList()
                //        }
                //    }
                //                };

                //                // Create the main card body
                //                var cardBody = new List<object>
                //{
                //    new
                //    {
                //        type = "TextBlock",
                //        text = "Response Summary",
                //        weight = "Bolder",
                //        size = "Large",
                //        spacing = "ExtraLarge",
                //        horizontalAlignment = "Center"
                //    },
                //    replyLinesContainer
                //};

                //                // Check if the document is available
                //                if (data.FileList != null && data.FileList.Any() && !containsWord)
                //                {
                //                    // Add references section title
                //                    var documentContainer = new
                //                    {
                //                        type = "Container",
                //                        spacing = "ExtraLarge",
                //                        separator = true,
                //                        items = new List<object>
                //        {
                //            new
                //            {
                //                type = "TextBlock",
                //                text = "References",
                //                weight = "Bolder",
                //                size = "Medium",
                //                color = "Accent",
                //                spacing = "Medium"
                //            }
                //        }
                //                    };

                //                    cardBody.Add(documentContainer);

                //                    int loopNumber = 1;
                //                    foreach (var file in data.FileList.Where(f => f.FileURL != null && f.FileName != null))
                //                    {
                //                        var columnSet = new
                //                        {
                //                            type = "ColumnSet",
                //                            spacing = "Medium",
                //                            columns = new List<object>
                //            {
                //                new
                //                {
                //                    type = "Column",
                //                    width = "stretch",
                //                    items = new List<object>
                //                    {
                //                        new
                //                        {
                //                            type = "TextBlock",
                //                            size = "Small",
                //                            text = $"📄 [Reference {loopNumber}: {file.FileName}]({file.FileURL})",
                //                            wrap = true,
                //                            color = "Good"
                //                        },
                //                        new
                //                        {
                //                            type = "TextBlock",
                //                            text = "Click above to view details",
                //                            wrap = true,
                //                            size = "Small",
                //                            spacing = "None",
                //                            color = "Accent"
                //                        }
                //                    }
                //                },
                //                new
                //                {
                //                    type = "Column",
                //                    width = "auto",
                //                    items = new List<object>
                //                    {
                //                        new
                //                        {
                //                            type = "Image",
                //                            url = "https://img.icons8.com/color/48/document.png",
                //                            altText = "Document",
                //                            width = "32px"
                //                        }
                //                    }
                //                }
                //            }
                //                        };

                //                        cardBody.Add(columnSet);
                //                        loopNumber++;
                //                    }
                //                }                

                //                // Create the final card content
                //                var cardContent = new
                //                {
                //                    type = "AdaptiveCard",
                //                    schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                //                    version = "1.4",
                //                    body = cardBody
                //                };

                //                // Serialize card content to JSON
                //                string cardJson = JsonConvert.SerializeObject(cardContent, Formatting.Indented);

                //                // Create and return the Attachment
                //                return new Attachment
                //                {
                //                    ContentType = "application/vnd.microsoft.card.adaptive",
                //                    Content = cardContent
                //                };



                //            // Add feedback section
                //            var feedbackContainer = new
                //            {
                //                type = "Container",
                //                spacing = "ExtraLarge",
                //                separator = true,
                //                items = new List<object>
                //{
                //    new
                //    {
                //        type = "TextBlock",
                //        text = "We value your feedback!",
                //        weight = "Bolder",
                //        size = "Medium",
                //        spacing = "Medium"
                //    },
                //    new
                //    {
                //        type = "Input.Text",
                //        id = "feedbackComment",
                //        placeholder = "Add your comments here...",
                //        spacing = "Small"
                //    },
                //    new
                //    {
                //        type = "ActionSet",
                //        spacing = "Medium",
                //        actions = new List<object>
                //        {
                //            new
                //            {
                //                type = "Action.Submit",
                //                title = "Submit Feedback",
                //                data = new { Command = "SubmitFeedback" }
                //            }
                //        }
                //    }
                //}
                //            };
                //            cardBody.Add(feedbackContainer);

                string imageUrl = _appSettings.AppDomainUrl + "/Images/searchResponse.jpg";

                var cardBody = new List<object>
{
                    // Hero Section with Image and Title
                    //new
                    //{
                    //    type = "Image",
                    //    url = imageUrl,
                    //    altText = "SOP Search Expert",
                    //    horizontalAlignment = "left",
                    //    spacing = "Medium"
                    //},

                    // Divider Section
                    //new
                    //{
                    //    type = "Container",
                    //    spacing = "Large",
                    //    separator = true,
                    //    items = new List<object>
                    //    {
                    //        new
                    //        {
                    //            type = "TextBlock",
                    //            text = "Here's the information you requested:",
                    //            wrap = true,
                    //            size = "Medium",
                    //            color = "Default"
                    //        }
                    //    }
                    //},

                    // Reply Lines Section
                    new
                    {
                        type = "Container",
                        style = "emphasis",
                        spacing = "Medium",
                        items = replyLines.Select(line => new
                        {
                            type = "TextBlock",
                            text = line,
                            wrap = true,
                            size = "Small",
                            spacing = "Small",
                            color = "Default"
                        }).ToList()
                    }
                };

                // References Section
                if (data.FileList != null && data.FileList.Any() && !containsWord)
                {
                    cardBody.Add(new
                    {
                        type = "Container",
                        spacing = "ExtraLarge",
                        separator = true,
                        items = new List<object>
                        {
                            new
                            {
                                type = "TextBlock",
                                text = "References",
                                weight = "Bolder",
                                size = "Medium",
                                color = "Accent",
                                spacing = "Medium"
                            },
                            //new
                            //{
                            //    type = "ColumnSet",
                            //    spacing = "Medium",
                            //    columns = data.FileList
                            //        .Where(f => f.FileURL != null && f.FileName != null)
                            //        .Select(file => new
                            //        {
                            //            type = "Column",
                            //            width = "stretch",
                            //            items = new List<object>
                            //            {
                            //                new
                            //                {
                            //                    type = "TextBlock",
                            //                    text = $"📄 [{file.FileName}]({file.FileURL})",
                            //                    wrap = true,
                            //                    color = "Good"
                            //                }
                            //            }
                            //        }).ToList()
                            //}
                            new
                            {
                                type = "Container",
                                spacing = "Medium",
                                items = data.FileList
                                    .Where(f => f.FileURL != null && f.FileName != null)
                                    .Select(file => new
                                    {
                                        type = "TextBlock",
                                        text = $"📄 [{file.FileName}]({file.FileURL})", // File link with name
                                        wrap = true,
                                        color = "Good",
                                        spacing = "Small"
                                    }).ToList()
                            }
                        }
                    });
                }

                // Feedback Section
                //cardBody.Add(new
                //{
                //    type = "Container",
                //    spacing = "ExtraLarge",
                //    separator = true,
                //    items = new List<object>
                //    {
                //        new
                //        {
                //            type = "TextBlock",
                //            text = "We'd love your feedback!",
                //            weight = "Bolder",
                //            size = "Medium",
                //            spacing = "Small"
                //        },
                //        new
                //        {
                //            type = "Input.Text",
                //            id = "feedbackComment",
                //            placeholder = "Share your thoughts...",
                //            spacing = "Medium"
                //        },
                //        new
                //        {
                //            type = "ColumnSet",
                //            spacing = "Medium",
                //            columns = new List<object>
                //            {
                //                new
                //                {
                //                    type = "Column",
                //                    width = "auto",
                //                    items = new List<object>
                //                    {
                //                        new
                //                        {
                //                            type = "Image",
                //                            url = "https://example.com/like-icon.png", // Replace with a 'like' icon URL
                //                            selectAction = new
                //                            {
                //                                type = "Action.Submit",
                //                                data = new { feedback = "like" }
                //                            },
                //                            altText = "Like"
                //                        }
                //                    }
                //                },
                //                new
                //                {
                //                    type = "Column",
                //                    width = "auto",
                //                    items = new List<object>
                //                    {
                //                        new
                //                        {
                //                            type = "Image",
                //                            url = "https://example.com/dislike-icon.png", // Replace with a 'dislike' icon URL
                //                            selectAction = new
                //                            {
                //                                type = "Action.Submit",
                //                                data = new { feedback = "dislike" }
                //                            },
                //                            altText = "Dislike"
                //                        }
                //                    }
                //                }
                //            }
                //        },
                //        new
                //        {
                //            type = "ActionSet",
                //            spacing = "Large",
                //            actions = new List<object>
                //            {
                //                new
                //                {
                //                    type = "Action.Submit",
                //                    title = "Submit Feedback",
                //                    style = "positive",
                //                    data = new { Command = "SubmitFeedback" }
                //                }
                //            }
                //        }
                //    }
                //});

                // Create the final card content
                var cardContent = new
                {
                    type = "AdaptiveCard",
                    schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                    version = "1.4",
                    body = cardBody,
                    msteams = new
                    {
                        width = "full" // Teams-specific property for full width
                    }
                };

                // Serialize card content to JSON
                string cardJson = JsonConvert.SerializeObject(cardContent, Formatting.Indented);

                // Create and return the Attachment
                return new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = cardContent
                };

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }

        }

        public Attachment CreateCard_IdleChatMessage_PersonalScope(string message, WebChatConversationModel data)
        {
            try
            {
                var actionButtons = new List<object>
                {
                    new { type = "Column", width = "150px", items = new List<object> { new { type = "ActionSet", actions = new List<object> { new { type = "Action.Submit", title = "End Conversation", tooltip = "End Conversation", style = "positive", data = new { action = "endChatAction", userId = data.User?.UserId } } } } } },
                };

                var cardContent = new
                {
                    type = "AdaptiveCard",
                    version = "1.5",
                    schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                    body = new List<object>
                    {
                        new { type = "TextBlock", text = message, weight = "Default", size = "Medium", wrap = true, horizontalAlignment = "Left", spacing = "Small" },
                        new { type = "Container", items = new List<object> { new { type = "ColumnSet", columns = actionButtons, horizontalAlignment = "Left" } } }
                    }
                };

                string cardJson = JsonConvert.SerializeObject(cardContent);
                Console.WriteLine(cardJson);

                return new Attachment { ContentType = "application/vnd.microsoft.card.adaptive", Content = cardContent };
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }
    }
}
