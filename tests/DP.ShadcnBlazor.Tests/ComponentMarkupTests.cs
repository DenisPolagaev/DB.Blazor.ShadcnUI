using Bunit;
using DP.ShadcnBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DP.ShadcnBlazor.Tests;

/// <summary>
/// Pins the markup contract: the <c>data-slot</c> attributes, the <c>data-variant</c>/<c>data-size</c>
/// values and the class lists must stay what shadcn/ui renders.
/// </summary>
public class ComponentMarkupTests : BunitContext
{
    public ComponentMarkupTests()
    {
        // The overlay components import their JS module in OnAfterRender; loose mode records the
        // calls instead of failing, and a module stub covers the ES-module import.
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule(ShadcnJs.ModulePath);
    }

    [Fact]
    public void Button_RendersShadcnSlotsAndDataAttributes()
    {
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Variant, ButtonVariant.Secondary)
            .Add(p => p.Size, ButtonSize.Sm)
            .AddChildContent("Save"));

        var button = cut.Find("button");
        Assert.Equal("button", button.GetAttribute("data-slot"));
        Assert.Equal("secondary", button.GetAttribute("data-variant"));
        Assert.Equal("sm", button.GetAttribute("data-size"));
        Assert.Equal("button", button.GetAttribute("type"));
        Assert.Contains("bg-secondary", button.ClassName);
        Assert.Contains("h-8", button.ClassName);
        Assert.Equal("Save", button.TextContent.Trim());
    }

    [Fact]
    public void Button_ConsumerClassOverridesDefaults()
    {
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Class, "h-12 px-8")
            .AddChildContent("Save"));

        var className = cut.Find("button").ClassName;
        Assert.Contains("h-12", className);
        Assert.DoesNotContain("h-9", className);
        Assert.Contains("px-8", className);
        Assert.DoesNotContain("px-4", className);
    }

    [Fact]
    public void Button_WithHrefRendersAnchor()
    {
        var cut = Render<Button>(parameters => parameters
            .Add(p => p.Href, "/docs")
            .AddChildContent("Docs"));

        var anchor = cut.Find("a");
        Assert.Equal("/docs", anchor.GetAttribute("href"));
        Assert.Equal("button", anchor.GetAttribute("data-slot"));
        Assert.Empty(cut.FindAll("button"));
    }

    [Fact]
    public void Badge_RendersVariant()
    {
        var cut = Render<Badge>(parameters => parameters
            .Add(p => p.Variant, BadgeVariant.Destructive)
            .AddChildContent("Failed"));

        var badge = cut.Find("span[data-slot='badge']");
        Assert.Equal("destructive", badge.GetAttribute("data-variant"));
        Assert.Contains("text-white", badge.ClassName);
    }

    [Fact]
    public void Card_RendersEverySlot()
    {
        var cut = Render<Card>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CardHeader>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(header =>
                {
                    header.OpenComponent<CardTitle>(0);
                    header.AddAttribute(1, "ChildContent", (RenderFragment)(title => title.AddContent(0, "Title")));
                    header.CloseComponent();
                    header.OpenComponent<CardDescription>(2);
                    header.AddAttribute(3, "ChildContent", (RenderFragment)(description => description.AddContent(0, "Hint")));
                    header.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<CardAction>(2);
                builder.CloseComponent();

                builder.OpenComponent<CardContent>(4);
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Body")));
                builder.CloseComponent();

                builder.OpenComponent<CardFooter>(6);
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(footer => footer.AddContent(0, "Footer")));
                builder.CloseComponent();
            }));

        Assert.NotNull(cut.Find("[data-slot='card']"));
        Assert.NotNull(cut.Find("[data-slot='card-header']"));
        Assert.Equal("Title", cut.Find("[data-slot='card-title']").TextContent.Trim());
        Assert.Equal("Hint", cut.Find("[data-slot='card-description']").TextContent.Trim());
        Assert.NotNull(cut.Find("[data-slot='card-action']"));
        Assert.Equal("Body", cut.Find("[data-slot='card-content']").TextContent.Trim());
        Assert.Equal("Footer", cut.Find("[data-slot='card-footer']").TextContent.Trim());
    }

    [Fact]
    public void Input_BindsValueOnInput()
    {
        string? value = null;
        var cut = Render<Input>(parameters => parameters
            .Add(p => p.Value, "start")
            .Add(p => p.ValueChanged, (string? next) => value = next)
            .Add(p => p.Placeholder, "Name"));

        var input = cut.Find("input[data-slot='input']");
        Assert.Equal("start", input.GetAttribute("value"));
        Assert.Equal("Name", input.GetAttribute("placeholder"));

        input.Input(new ChangeEventArgs { Value = "next" });
        Assert.Equal("next", value);
    }

    [Fact]
    public void Checkbox_TogglesCheckedAndAriaState()
    {
        var checkedState = false;
        var cut = Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, false)
            .Add(p => p.CheckedChanged, value => checkedState = value));

        var button = cut.Find("button[data-slot='checkbox']");
        Assert.Equal("unchecked", button.GetAttribute("data-state"));
        Assert.Equal("false", button.GetAttribute("aria-checked"));
        Assert.Empty(cut.FindAll("[data-slot='checkbox-indicator']"));

        cut.Find("button").Click();

        Assert.True(checkedState);
        cut.Find("button[data-slot='checkbox']").Click();
        Assert.False(checkedState);
    }

    [Fact]
    public void Tabs_SwitchesPanelAndAriaLinks()
    {
        var cut = Render<Tabs>(parameters => parameters
            .Add(p => p.DefaultValue, "one")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<TabsList>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(list =>
                {
                    list.OpenComponent<TabsTrigger>(0);
                    list.AddAttribute(1, "Value", "one");
                    list.AddAttribute(2, "ChildContent", (RenderFragment)(content => content.AddContent(0, "One")));
                    list.CloseComponent();

                    list.OpenComponent<TabsTrigger>(2);
                    list.AddAttribute(3, "Value", "two");
                    list.AddAttribute(4, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Two")));
                    list.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<TabsContent>(2);
                builder.AddAttribute(3, "Value", "one");
                builder.AddAttribute(4, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Panel one")));
                builder.CloseComponent();

                builder.OpenComponent<TabsContent>(5);
                builder.AddAttribute(6, "Value", "two");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Panel two")));
                builder.CloseComponent();
            }));

        var triggers = cut.FindAll("[data-slot='tabs-trigger']");
        Assert.Equal(2, triggers.Count);
        Assert.Equal("active", triggers[0].GetAttribute("data-state"));
        Assert.Equal("inactive", triggers[1].GetAttribute("data-state"));

        var firstPanel = cut.Find("[data-slot='tabs-content'][data-state='active']");
        Assert.Equal("Panel one", firstPanel.TextContent.Trim());
        Assert.Equal(triggers[0].GetAttribute("id"), firstPanel.GetAttribute("aria-labelledby"));

        triggers[1].Click();

        var active = cut.Find("[data-slot='tabs-content'][data-state='active']");
        Assert.Equal("Panel two", active.TextContent.Trim());
    }

    [Fact]
    public void Accordion_SingleExpandsOneItemAtATime()
    {
        var cut = Render<Accordion>(parameters => parameters
            .Add(p => p.Type, AccordionType.Single)
            .Add(p => p.Collapsible, true)
            .AddChildContent(builder =>
            {
                foreach (var (value, index) in new[] { "a", "b" }.Select((value, index) => (value, index)))
                {
                    builder.OpenComponent<AccordionItem>(index * 4);
                    builder.AddAttribute((index * 4) + 1, "Value", value);
                    builder.AddAttribute((index * 4) + 2, "ChildContent", (RenderFragment)(item =>
                    {
                        item.OpenComponent<AccordionTrigger>(0);
                        item.AddAttribute(1, "ChildContent", (RenderFragment)(trigger => trigger.AddContent(0, "Trigger " + value)));
                        item.CloseComponent();

                        item.OpenComponent<AccordionContent>(2);
                        item.AddAttribute(3, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Content " + value)));
                        item.CloseComponent();
                    }));
                    builder.CloseComponent();
                }
            }));

        var triggers = cut.FindAll("[data-slot='accordion-trigger']");
        Assert.All(triggers, trigger => Assert.Equal("false", trigger.GetAttribute("aria-expanded")));

        triggers[0].Click();
        Assert.Equal("open", cut.FindAll("[data-slot='accordion-item']")[0].GetAttribute("data-state"));
        Assert.Equal("true", cut.FindAll("[data-slot='accordion-trigger']")[0].GetAttribute("aria-expanded"));

        cut.FindAll("[data-slot='accordion-trigger']")[1].Click();
        Assert.Equal("closed", cut.FindAll("[data-slot='accordion-item']")[0].GetAttribute("data-state"));
        Assert.Equal("open", cut.FindAll("[data-slot='accordion-item']")[1].GetAttribute("data-state"));
    }

    [Fact]
    public void Dialog_RendersNothingUntilOpened()
    {
        var cut = Render<Dialog>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.OpenComponent<Button>(0);
                    trigger.AddAttribute(1, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Open")));
                    trigger.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<DialogContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                {
                    content.OpenComponent<DialogHeader>(0);
                    content.AddAttribute(1, "ChildContent", (RenderFragment)(header =>
                    {
                        header.OpenComponent<DialogTitle>(0);
                        header.AddAttribute(1, "ChildContent", (RenderFragment)(title => title.AddContent(0, "Title")));
                        header.CloseComponent();
                    }));
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        Assert.Empty(cut.FindAll("[data-slot='dialog-content']"));
        Assert.Equal("false", cut.Find("[data-slot='dialog-trigger']").GetAttribute("aria-expanded"));

        // Opening mounts the panel; the JS module is not available in bUnit, and the components
        // deliberately tolerate that (they still render the correct markup).
        cut.Find("button").Click();

        Assert.NotNull(cut.Find("[data-slot='dialog-content']"));
        Assert.Equal("true", cut.Find("[data-slot='dialog-trigger']").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Separator_IsDecorativeByDefault()
    {
        var cut = Render<Separator>();
        var separator = cut.Find("[data-slot='separator']");
        Assert.Equal("none", separator.GetAttribute("role"));
        Assert.Equal("horizontal", separator.GetAttribute("data-orientation"));
    }

    [Fact]
    public void Progress_TranslatesTheIndicator()
    {
        var cut = Render<Progress>(parameters => parameters.Add(p => p.Value, 25));
        var indicator = cut.Find("[data-slot='progress-indicator']");
        Assert.Contains("translateX(-75%)", indicator.GetAttribute("style"));
        Assert.Equal("25", cut.Find("[data-slot='progress']").GetAttribute("aria-valuenow"));
    }
}
