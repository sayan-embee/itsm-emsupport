using AdaptiveCards;
using AdaptiveCards.Templating;
using Common.Layer.Models;
using Common.Layer.Models.AdaptiveCard;
using Common.Layer.Models.AppSettings;
using Internal.TeamsBot.Layer.ExceptionLog;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace Internal.TeamsBot.Layer.Services.AdaptiveCards
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
            this._appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
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

            var mainDirectoryPath = Directory.GetCurrentDirectory() + @"\Documents_Internal";
            var cardJsonFilePath = Path.Combine(mainDirectoryPath, $".\\AdaptiveCards\\{jsonTemplateFileName}");
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

        public Attachment? CreateCard_UserMessage_PersonalScope(UserMessageModel data)
        {
            try
            {
                string greetingMsg = "! \n I'm SOP Search Expert, Embee's very own Gen AI virtual assistant. I'm here to assist with all your SOP related queries.";
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

                string imageUrl = _appSettings.AppDomainUrl+"/Images/searchResponse.jpg";

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
    }
}
