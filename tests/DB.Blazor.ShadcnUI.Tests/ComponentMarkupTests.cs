using Bunit;
using DB.Blazor.ShadcnUI.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DB.Blazor.ShadcnUI.Tests;

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
    public void Button_AlwaysRendersTheNativeButton()
    {
        var cut = Render<Button>(parameters => parameters
            .AddUnmatched("id", "save")
            .AddChildContent("Save"));

        var button = cut.Find("button[data-slot='button']");
        // shadcn/ui spreads its props onto the native button: `id` is a plain HTML attribute,
        // there is no `href` prop (links use buttonVariants on an <a>).
        Assert.Equal("save", button.GetAttribute("id"));
        Assert.Empty(cut.FindAll("a"));
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

    [Fact]
    public void Select_RendersTheShadcnListboxContract()
    {
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.DefaultValue, "banana")
            .AddChildContent(SelectTree()));

        var trigger = cut.Find("[data-slot='select-trigger']");
        Assert.Equal("combobox", trigger.GetAttribute("role"));
        Assert.Equal("closed", trigger.GetAttribute("data-state"));
        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));
        Assert.Equal("default", trigger.GetAttribute("data-size"));
        Assert.Null(trigger.GetAttribute("data-placeholder"));
        Assert.Equal("select-value", trigger.QuerySelector("[data-slot='select-value']")?.GetAttribute("data-slot"));

        var value = cut.Find("[data-slot='select-value']");
        Assert.Equal("Banana", value.TextContent.Trim());

        var content = cut.Find("[data-slot='select-content']");
        Assert.Equal("listbox", content.GetAttribute("role"));
        Assert.True(content.HasAttribute("hidden"));

        var items = cut.FindAll("[data-slot='select-item']");
        Assert.Equal(2, items.Count);
        Assert.Equal("unchecked", items[0].GetAttribute("data-state"));
        Assert.Equal("checked", items[1].GetAttribute("data-state"));
        Assert.Equal("Banana", items[1].QuerySelector("span:last-child")?.TextContent.Trim());
    }

    [Fact]
    public void Select_ShowsThePlaceholderAndSmallSize()
    {
        var cut = Render<Select>(parameters => parameters.AddChildContent(SelectTree(size: SelectSize.Sm)));

        var trigger = cut.Find("[data-slot='select-trigger']");
        Assert.Equal("sm", trigger.GetAttribute("data-size"));
        Assert.Contains("data-placeholder", trigger.OuterHtml);
        Assert.Equal("Select a fruit", cut.Find("[data-slot='select-value']").TextContent.Trim());
    }

    [Fact]
    public void Select_ChoosesAnItemAndCloses()
    {
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.DefaultValue, "apple")
            .AddChildContent(SelectTree()));

        cut.FindAll("[data-slot='select-item']")[1].Click();

        var trigger = cut.Find("[data-slot='select-trigger']");
        Assert.Equal("closed", trigger.GetAttribute("data-state"));
        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));
        Assert.Equal("Banana", cut.Find("[data-slot='select-value']").TextContent.Trim());
        Assert.Equal("checked", cut.FindAll("[data-slot='select-item']")[1].GetAttribute("data-state"));
    }

    [Fact]
    public void Select_DisabledDoesNotOpenOrSelect()
    {
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.DefaultValue, "apple")
            .AddChildContent(SelectTree()));

        var trigger = cut.Find("[data-slot='select-trigger']");
        Assert.True(trigger.HasAttribute("disabled"));
        Assert.Equal("", trigger.GetAttribute("data-disabled"));

        trigger.Click();
        Assert.Equal("closed", trigger.GetAttribute("data-state"));

        cut.FindAll("[data-slot='select-item']")[1].Click();
        Assert.Equal("Apple", cut.Find("[data-slot='select-value']").TextContent.Trim());
    }

    [Fact]
    public void SelectItem_DisabledStaysUnselectable()
    {
        var cut = Render<Select>(parameters => parameters
            .Add(p => p.DefaultValue, "apple")
            .AddChildContent(SelectTree(disabledValue: "banana")));

        cut.FindAll("[data-slot='select-item']")[1].Click();

        Assert.Equal("Apple", cut.Find("[data-slot='select-value']").TextContent.Trim());
        Assert.Equal("", cut.FindAll("[data-slot='select-item']")[1].GetAttribute("data-disabled"));
    }

    private static RenderFragment SelectTree(SelectSize size = SelectSize.Default, string? disabledValue = null) => builder =>
    {
        builder.OpenComponent<SelectTrigger>(0);
        builder.AddAttribute(1, "Size", size);
        builder.AddAttribute(2, "ChildContent", (RenderFragment)(trigger =>
        {
            trigger.OpenComponent<SelectValue>(0);
            trigger.AddAttribute(1, "Placeholder", "Select a fruit");
            trigger.CloseComponent();
        }));
        builder.CloseComponent();

        builder.OpenComponent<SelectContent>(3);
        builder.AddAttribute(4, "ChildContent", (RenderFragment)(content =>
        {
            foreach (var (value, label, index) in new[] { ("apple", "Apple", 0), ("banana", "Banana", 1) })
            {
                content.OpenComponent<SelectItem>(index * 4);
                content.AddAttribute((index * 4) + 1, "Value", value);
                content.AddAttribute((index * 4) + 2, "Disabled", value == disabledValue);
                content.AddAttribute((index * 4) + 3, "ChildContent", (RenderFragment)(item => item.AddContent(0, label)));
                content.CloseComponent();
            }
        }));
        builder.CloseComponent();
    };

    [Fact]
    public void Command_RendersThePaletteContract()
    {
        var cut = Render<Command>(parameters => parameters
            .Add(p => p.Label, "Commands")
            .AddChildContent(CommandTree()));

        var root = cut.Find("[data-slot='command']");
        Assert.Equal("true", root.GetAttribute("data-should-filter"));
        Assert.Equal("false", root.GetAttribute("data-loop"));

        var wrapper = cut.Find("[data-slot='command-input-wrapper']");
        Assert.True(wrapper.HasAttribute("cmdk-input-wrapper"));

        var input = cut.Find("[data-slot='command-input']");
        Assert.True(input.HasAttribute("cmdk-input"));
        Assert.Equal("combobox", input.GetAttribute("role"));
        Assert.Equal("true", input.GetAttribute("aria-expanded"));

        var list = cut.Find("[data-slot='command-list']");
        Assert.Equal("listbox", list.GetAttribute("role"));
        Assert.Equal("Commands", list.GetAttribute("aria-label"));
        Assert.Equal(list.GetAttribute("id"), input.GetAttribute("aria-controls"));

        var group = cut.Find("[data-slot='command-group']");
        Assert.Equal("group", group.GetAttribute("role"));
        Assert.True(group.HasAttribute("cmdk-group"));
        var heading = group.QuerySelector("[cmdk-group-heading]");
        Assert.NotNull(heading);
        Assert.Equal("Suggestions", heading!.TextContent.Trim());
        Assert.Equal(heading.GetAttribute("id"), group.GetAttribute("aria-labelledby"));

        var items = cut.FindAll("[data-slot='command-item']");
        Assert.Equal(2, items.Count);
        Assert.Equal("option", items[0].GetAttribute("role"));
        Assert.Equal("-1", items[0].GetAttribute("tabindex"));
        Assert.Equal("calendar", items[0].GetAttribute("data-value"));
        Assert.Equal("false", items[0].GetAttribute("data-selected"));
        Assert.Equal("false", items[0].GetAttribute("data-disabled"));
        Assert.Contains("display: none", cut.Find("[data-slot='command-empty']").GetAttribute("style"));
        Assert.Equal("⌘C", cut.Find("[data-slot='command-shortcut']").TextContent.Trim());
    }

    [Fact]
    public void CommandItem_InvokesOnSelectAndSkipsDisabled()
    {
        var selected = 0;
        var cut = Render<Command>(parameters => parameters
            .AddChildContent(CommandTree(disabledValue: "settings", onSelect: () => selected++)));

        var items = cut.FindAll("[data-slot='command-item']");
        items[1].Click();
        Assert.Equal(0, selected);

        items[0].Click();
        Assert.Equal(1, selected);
    }

    [Fact]
    public void CommandDialog_RendersThePaletteInsideTheDialog()
    {
        var cut = Render<CommandDialog>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent(CommandTree()));

        var content = cut.Find("[data-slot='dialog-content']");
        Assert.Contains("p-0", content.ClassName);
        Assert.DoesNotContain("p-6", content.ClassName);
        Assert.NotNull(cut.Find("[data-slot='command']"));
        Assert.NotNull(cut.Find("[data-slot='command-input']"));

        var title = cut.Find("[data-slot='dialog-title']");
        Assert.Equal("Command Palette", title.TextContent.Trim());
        Assert.Equal(title.GetAttribute("id"), content.GetAttribute("aria-labelledby"));
        Assert.Equal("Search for a command to run...",
            cut.Find("[data-slot='dialog-description']").TextContent.Trim());
        Assert.Contains("sr-only", cut.Find("[data-slot='dialog-header']").ClassName);
    }

    private RenderFragment CommandTree(string? disabledValue = null, Action? onSelect = null) => builder =>
    {
        builder.OpenComponent<CommandInput>(0);
        builder.AddAttribute(1, "Placeholder", "Type a command or search...");
        builder.CloseComponent();

        builder.OpenComponent<CommandList>(2);
        builder.AddAttribute(3, "ChildContent", (RenderFragment)(list =>
        {
            list.OpenComponent<CommandEmpty>(0);
            list.AddAttribute(1, "ChildContent", (RenderFragment)(empty => empty.AddContent(0, "No results found.")));
            list.CloseComponent();

            list.OpenComponent<CommandGroup>(2);
            list.AddAttribute(3, "Heading", "Suggestions");
            list.AddAttribute(4, "ChildContent", (RenderFragment)(group =>
            {
                var index = 0;
                foreach (var (value, label, shortcut) in new[]
                {
                    ("calendar", "Calendar", "⌘C"),
                    ("settings", "Settings", "⌘S"),
                })
                {
                    group.OpenComponent<CommandItem>(index * 5);
                    group.AddAttribute((index * 5) + 1, "Value", value);
                    group.AddAttribute((index * 5) + 2, "Disabled", value == disabledValue);
                    group.AddAttribute((index * 5) + 3, "OnSelect",
                        EventCallback.Factory.Create(this, () => onSelect?.Invoke()));
                    group.AddAttribute((index * 5) + 4, "ChildContent", (RenderFragment)(item =>
                    {
                        item.AddContent(0, label);
                        item.OpenComponent<CommandShortcut>(1);
                        item.AddAttribute(2, "ChildContent", (RenderFragment)(s => s.AddContent(0, shortcut)));
                        item.CloseComponent();
                    }));
                    group.CloseComponent();
                    index++;
                }
            }));
            list.CloseComponent();

            list.OpenComponent<CommandSeparator>(5);
            list.CloseComponent();
        }));
        builder.CloseComponent();
    };
}
