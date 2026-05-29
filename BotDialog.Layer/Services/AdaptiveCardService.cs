using Common.Layer.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using Common.Layer.Models.WebChatBot;

namespace BotDialog.Layer.Services
{
    public class AdaptiveCardService
    {
        //public Attachment GenerateCategoryAdaptiveCard(List<AdaptiveCardModel> cardModels)
        //{
        //    var cardContent = new
        //    {
        //        type = "AdaptiveCard",
        //        body = new List<object>
        //    {
        //        new
        //        {
        //            type = "TextBlock",
        //            text = "Please select a support category:",
        //            weight = "Bolder",
        //            size = "Medium"
        //        },
        //        new
        //        {
        //            type = "Input.ChoiceSet",
        //            id = "categoryChoice",
        //            style = "expanded",
        //            choices = cardModels.Select(c => new
        //            {
        //                title = c.Name,
        //                value = c.Code
        //            }).ToList()
        //        }
        //    },
        //        actions = new List<object>
        //        {
        //            new
        //            {
        //                type = "Action.Submit",
        //                title = "Submit",
        //                data = new { action = "submitCategory" }
        //            }
        //        },
        //        schema = "http://adaptivecards.io/schemas/adaptive-card",
        //        version = "1.3"
        //    };

        //    return new Attachment
        //    {
        //        ContentType = "application/vnd.microsoft.card.adaptive",
        //        Content = cardContent
        //    };
        //}

        public Attachment GenerateCategoryAdaptiveCard(List<AdaptiveCardModel> cardModels)
        {
            var optionContainers = new List<object>();
            var actions = new List<object>();

            foreach (var model in cardModels)
            {
                if (model.Id > 0) // Normal subcategories
                {
                    optionContainers.Add(new
                    {
                        type = "Container",
                        backgroundColor = "#E3F2FD",
                        items = new List<object>
                        {
                            new
                            {
                                type = "TextBlock",
                                text = model.Name,
                                wrap = true,
                                size = "Default",
                                color = "Default",
                                horizontalAlignment = "Left",
                                weight = "Bolder"
                            }
                        },
                        selectAction = new
                        {
                            type = "Action.Submit",
                            title = model.Name,
                            data = new { action = "submitCategory", categoryId = model.Id }
                        },
                        style = "accent"
                    });
                }
                else // Special cases for actions
                {
                    actions.Add(new
                    {
                        type = "Column",
                        width = "stretch",
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
                                        title = model.Name,
                                        data = new { action = "submitCategory", categoryId = model.Id },
                                        style = model.Id == 0 ? "destructive" : model.Id == -3 ? "positive" : null
                                    }
                                }
                            }
                        }
                    });
                }
            }

            // Create the Adaptive Card content
            var cardContent = new
            {
                type = "AdaptiveCard",
                version = "1.5",
                body = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = "Let us know what kind of support you need:",
                        weight = "Bolder",
                        size = "Large",
                        wrap = true,
                        horizontalAlignment = "Left",
                        spacing = "Small",
                        color = "Accent",
                        fontType = "Default"
                    }
                }
                .Concat(optionContainers)
                .Append(new  // Action Buttons in a ColumnSet
                {
                    type = "Container",
                    items = new List<object>
                    {
                        new
                        {
                            type = "ColumnSet",
                            columns = actions,
                            horizontalAlignment = "Center"
                        }
                    },
                    horizontalAlignment = "Center",
                    style = "emphasis"
                }).ToList(),
                schema = "http://adaptivecards.io/schemas/adaptive-card.json"
            };

            string cardJson = JsonConvert.SerializeObject(cardContent);
            Console.WriteLine(cardJson);

            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardContent
            };
        }

        //public Attachment GenerateSubCategoryAdaptiveCard(List<AdaptiveCardModel> subCategoryModels)
        //{
        //    var cardContent = new
        //    {
        //        type = "AdaptiveCard",
        //        body = new List<object>
        //        {
        //            new
        //            {
        //                type = "TextBlock",
        //                text = "Please select a support sub-category:",
        //                weight = "Bolder",
        //                size = "Medium"
        //            },
        //            new
        //            {
        //                type = "Input.ChoiceSet",
        //                id = "subCategoryChoice",
        //                style = "expanded",
        //                choices = subCategoryModels.Select(c => new
        //                {
        //                    title = c.Name,
        //                    value = c.Code
        //                }).ToList()
        //            }
        //        },
        //        actions = new List<object>
        //        {
        //            new
        //            {
        //                type = "Action.Submit",
        //                title = "Submit",
        //                data = new { action = "submitSubCategory" }
        //            }
        //        },
        //        schema = "http://adaptivecards.io/schemas/adaptive-card",
        //        version = "1.3"
        //        };

        //    return new Attachment
        //    {
        //        ContentType = "application/vnd.microsoft.card.adaptive",
        //        Content = cardContent
        //    };
        //}


        public Attachment GenerateSubCategoryAdaptiveCard_Bak(List<AdaptiveCardModel> subCategoryModels)
        {
            // Create the list of containers for the subcategory options dynamically based on subCategoryModels
            var optionContainers = new List<object>();

            foreach (var model in subCategoryModels)
            {
                optionContainers.Add(new
                {
                    type = "Container",
                    style = "accent",
                    items = new List<object>
                    {
                        new
                        {
                            type = "TextBlock",
                            text = model.Name, // Use dynamic subcategory name
                            wrap = true,
                            size = "Medium",
                            color = "Accent",
                            horizontalAlignment = "Center",
                            weight = "Bolder",
                        }
                    },
                    selectAction = new
                    {
                        type = "Action.Submit",
                        title = $"Select {model.Name}", // Title dynamically based on subcategory name
                        data = new { action = "submitSubCategory", subCategoryId = model.Id } // Pass dynamic code
                    },
                    spacing = "Small"
                });
            }

            // Create the Adaptive Card content
            var cardContent = new
            {
                type = "AdaptiveCard",
                version = "1.5",
                body = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = "Let us know what kind of support you need:",
                        weight = "Bolder",
                        size = "Large",
                        wrap = true,
                        horizontalAlignment = "Center",
                        spacing = "Small"
                    }
                }
            };

            // Add the dynamically created subcategory options directly to the body
            cardContent.body.AddRange(optionContainers);

            string cardJson = JsonConvert.SerializeObject(cardContent);
            Console.WriteLine(cardJson);

            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardContent
            };
        }

        public Attachment GenerateSubCategoryAdaptiveCard(List<AdaptiveCardModel> subCategoryModels)
        {
            var optionContainers = new List<object>();
            var actions = new List<object>();

            foreach (var model in subCategoryModels)
            {
                if (model.Id > 0) // Normal subcategories
                {
                    optionContainers.Add(new
                    {
                        type = "Container",
                        backgroundColor = "#E3F2FD",
                        items = new List<object>
                        {
                            new
                            {
                                type = "TextBlock",
                                text = model.Name,
                                wrap = true,
                                size = "Default",
                                color = "Default",
                                horizontalAlignment = "Left",
                                weight = "Bolder"
                            }
                        },
                        selectAction = new
                        {
                            type = "Action.Submit",
                            title = model.Name,
                            data = new { action = "submitSubCategory", subCategoryId = model.Id }
                        },
                        style = "accent"
                    });
                }
                else // Special cases for actions
                {
                    actions.Add(new
                    {
                        type = "Column",
                        width = "stretch",
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
                                        title = model.Name,
                                        data = new { action = "submitSubCategory", subCategoryId = model.Id },
                                        style = model.Id == 0 ? "destructive" : model.Id == -3 ? "positive" : null
                                    }
                                }
                            }
                        }
                    });
                }
            }

            // Create the Adaptive Card content
            var cardContent = new
            {
                type = "AdaptiveCard",
                version = "1.5",
                body = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = "Let us know what kind of support you need:",
                        weight = "Bolder",
                        size = "Large",
                        wrap = true,
                        horizontalAlignment = "Left",
                        spacing = "Small",
                        color = "Accent",
                        fontType = "Default"
                    }
                }
                .Concat(optionContainers)
                .Append(new  // Action Buttons in a ColumnSet
                {
                    type = "Container",
                    items = new List<object>
                    {
                        new
                        {
                            type = "ColumnSet",
                            columns = actions,
                            horizontalAlignment = "Center"
                        }
                    },
                    horizontalAlignment = "Center",
                    style = "emphasis"
                }).ToList(),
                schema = "http://adaptivecards.io/schemas/adaptive-card.json"
            };

            string cardJson = JsonConvert.SerializeObject(cardContent);
            Console.WriteLine(cardJson);

            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardContent
            };
        }


        //public Attachment GenerateFinalOptionsCard(List<AdaptiveCardModel> finalOptions)
        //{
        //    // Build the list of choices dynamically based on finalOptions
        //    var choices = finalOptions.Select(option => new
        //    {
        //        title = option.Name,
        //        value = option.Code
        //    }).ToList();

        //    var cardContent = new
        //    {
        //        type = "AdaptiveCard",
        //        body = new List<object>
        //        {
        //            new
        //            {
        //                type = "TextBlock",
        //                text = "What would you like to do next?",
        //                weight = "Bolder",
        //                size = "Medium"
        //            },
        //            new
        //            {
        //                type = "Input.ChoiceSet",
        //                id = "finalChoice",
        //                style = "expanded",
        //                choices = choices
        //            }
        //        },
        //        actions = new List<object>
        //        {
        //            new
        //            {
        //                type = "Action.Submit",
        //                title = "Submit",
        //                data = new { action = "finalSubmit" }
        //            }
        //        },
        //        schema = "http://adaptivecards.io/schemas/adaptive-card",
        //        version = "1.3"
        //    };

        //    return new Attachment
        //    {
        //        ContentType = "application/vnd.microsoft.card.adaptive",
        //        Content = cardContent
        //    };
        //}

        public Attachment GenerateFinalOptionsCard_Bak(List<AdaptiveCardModel> finalOptions)
        {
            // Build the list of containers for each choice dynamically based on finalOptions
            var optionContainers = finalOptions.Select(option => new
            {
                type = "Container",
                style = "accent", // You can change this style as needed
                items = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = option.Name, // Use dynamic option name
                        wrap = true,
                        size = "Medium",
                        weight = "Bolder",
                        color = "Accent",
                        horizontalAlignment = "Center"
                    }
                },
                selectAction = new
                {
                    type = "Action.Submit",
                    title = $"Select {option.Name}", // Title dynamically based on option name
                    data = new { action = "finalSubmit", optionId = option.Id } // Pass dynamic code
                },
                spacing = "Small" // Adjust spacing as needed
            }).ToList();

            // Create the Adaptive Card content
            var cardContent = new
            {
                type = "AdaptiveCard",
                version = "1.5", // Updated to 1.5 for consistency
                body = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = "Choose the option that best describes your needs,",
                        weight = "Bolder",
                        size = "Large",
                        wrap = true,
                        horizontalAlignment = "Center",
                        spacing = "Small"
                    },
                    new
                    {
                        type = "TextBlock",
                        text = "or select 'Other' to specify.",
                        weight = "Bolder",
                        size = "Large",
                        wrap = true,
                        horizontalAlignment = "Center",
                        spacing = "Small"
                    }
                }
            };

            // Add the dynamically created option containers directly to the body
            cardContent.body.AddRange(optionContainers);

            // Create the final card content with actions
            var finalCardContent = new
            {
                type = "AdaptiveCard",
                version = "1.5",
                cardContent.body,
                schema = "http://adaptivecards.io/schemas/adaptive-card.json"
            };

            string cardJson = JsonConvert.SerializeObject(finalCardContent);
            Console.WriteLine(cardJson);

            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = finalCardContent
            };
        }

        public Attachment GenerateFinalOptionsCard_Bak2(List<AdaptiveCardModel> finalOptions)
        {
            // Separate options (Id > 0) and actions (Id <= 0)
            var optionContainers = finalOptions
                .Where(option => option.Id > 0) // Only include options with Id > 0
                .Select(option => new
                {
                    type = "Container",
                    style = "accent",
                    items = new List<object>
                    {
                    new
                    {
                        type = "TextBlock",
                        text = option.Name,
                        wrap = true,
                        size = "Medium",
                        weight = "Bolder",
                        color = "Accent",
                        horizontalAlignment = "Center"
                    }
                    },
                    selectAction = new
                    {
                        type = "Action.Submit",
                        title = $"Select {option.Name}",
                        data = new { action = "finalSubmit", optionId = option.Id }
                    },
                    spacing = "Small"
                }).ToList();

            // Process action sets (Id <= 0)
            var actionButtons = finalOptions
                .Where(option => option.Id <= 0) // Only include action sets with Id <= 0
                .Select(action => new
                {
                    type = "Column",
                    width = "stretch",
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
                                title = action.Name,
                                data = new { action = action.Id == 0 ? "endChat" : action.Id == -3 ? "others" : "goBack" },
                                style = action.Id == 0 ? "destructive" : "positive" // "End Chat" should be destructive
                            }
                        }
                    }
                    }
                }).ToList();

            // Create the Adaptive Card content
            var cardContent = new
            {
                type = "AdaptiveCard",
                version = "1.6",
                body = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = "Choose the option that best describes your needs,",
                        weight = "Bolder",
                        size = "Large",
                        wrap = true,
                        horizontalAlignment = "Center",
                        spacing = "Small"
                    },
                    new
                    {
                        type = "TextBlock",
                        text = "or select 'Other' to specify.",
                        weight = "Bolder",
                        size = "Large",
                        wrap = true,
                        horizontalAlignment = "Center",
                        spacing = "Small"
                    }
                }
                .Concat(optionContainers) // Add dynamically created options
                .Append(new  // Action Buttons in a ColumnSet
                {
                    type = "Container",
                    items = new List<object>
                    {
                    new
                    {
                        type = "ColumnSet",
                        columns = actionButtons // Add dynamically created action buttons
                    }
                    }
                }).ToList()
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

        public Attachment GenerateFinalOptionsCard(List<AdaptiveCardModel> finalOptions)
        {
            // Separate options (Id > 0) and actions (Id <= 0)
            var optionContainers = finalOptions
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
                        data = new { action = "finalSubmit", optionId = option.Id }
                    },
                    style = "accent"
                }).ToList();

                // Process action sets (Id <= 0)
                var actionButtons = finalOptions
                    .Where(option => option.Id <= 0)
                    .Select(action => new
                    {
                        type = "Column",
                        width = "stretch",
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
                                        title = action.Name,
                                        data = new { action = action.Id == 0 ? "endChat" : action.Id == -3 ? "others" : "goBack" },
                                        style = action.Id == 0 ? "destructive" : action.Id == -3 ? "positive" : null
                                    }
                                }
                            }
                        }
                    }).ToList();

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
                            text = "Select an option that best describes your needs Or choose 'Others' to specify your request.",
                            weight = "Bolder",
                            size = "Medium",
                            horizontalAlignment = "Left",
                            wrap = true,
                            color = "Accent",
                            fontType = "Default"
                        }
                    }
                    .Concat(optionContainers)
                    .Append(new  // Action Buttons in a ColumnSet
                    {
                        type = "Container",
                        items = new List<object>
                        {
                            new
                            {
                                type = "ColumnSet",
                                columns = actionButtons,
                                horizontalAlignment = "Center"
                            }
                        },
                        horizontalAlignment = "Center",
                        style = "emphasis"
                    }).ToList(),
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

        public Attachment GenerateFeedbackCard()
        {
            var cardContent = new
            {
                type = "AdaptiveCard",
                version = "1.3",
                body = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = "How would you rate your experience?",
                        weight = "Bolder",
                        size = "Medium"
                    },
                    new
                    {
                        type = "Input.ChoiceSet",
                        id = "selectedOption",
                        style = "expanded",
                        choices = new[]
                        {
                            new { title = "Excellent", value = "5" },
                            new { title = "Good", value = "4" },
                            new { title = "Average", value = "3" },
                            new { title = "Poor", value = "2" },
                            new { title = "Very Poor", value = "1" }
                        }
                    },
                    new
                    {
                        type = "TextBlock",
                        text = "Would you like to provide additional feedback? (optional):",
                        wrap = true
                    },
                    new
                    {
                        type = "Input.Text",
                        id = "additionalFeedback",
                        placeholder = "Enter your feedback here (optional)",
                        isMultiline = true,
                        isRequired = false
                    }
                },
                actions = new List<object>
                {
                    new
                    {
                        type = "Action.Submit",
                        title = "Submit Feedback",
                        data = new { action = "submitFeedback" }
                    }
                },
                schema = "http://adaptivecards.io/schemas/adaptive-card"
            };

            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardContent
            };
        }

        public Attachment GenerateRatingCard(List<WebChatFeedbackOptionsModel> feedbackOptions)
        {
            var cardContent = new
            {
                type = "AdaptiveCard",
                version = "1.3",
                body = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = "How would you rate your experience?",
                        weight = "Bolder",
                        size = "Medium"
                    }
                },
                actions = feedbackOptions.Select(option => new
                {
                    type = "Action.Submit",
                    title = option.Name,
                    data = new { action = "submitRating", rating = option.Id }
                }).ToList(),
                schema = "http://adaptivecards.io/schemas/adaptive-card"
            };

            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardContent
            };
        }

        public Attachment GenerateAdditionalFeedbackCard()
        {
            var cardContent = new
            {
                type = "AdaptiveCard",
                version = "1.3",
                body = new List<object>
                {
                    new
                    {
                        type = "TextBlock",
                        text = "Would you like to provide additional feedback? (optional):",
                        wrap = true
                    },
                    new
                    {
                        type = "Input.Text",
                        id = "additionalFeedback",
                        placeholder = "Enter your feedback here (optional)",
                        isMultiline = true
                    }
                },
                actions = new List<object>
                {
                    new
                    {
                        type = "Action.Submit",
                        title = "Submit Feedback",
                        data = new { action = "submitAdditionalFeedback" }
                    }
                },
                schema = "http://adaptivecards.io/schemas/adaptive-card"
            };

            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardContent
            };
        }

    }
}
