namespace DB.Blazor.ShadcnUI;

/// <summary>
/// A focused port of <c>tailwind-merge</c>: it resolves the conflicts that shadcn/ui components
/// actually produce when a consumer overrides a class (<c>h-9</c> vs <c>h-12</c>,
/// <c>px-4</c> vs <c>px-8</c>, <c>bg-primary</c> vs <c>bg-secondary</c>, …).
/// </summary>
/// <remarks>
/// Conflicts are resolved per variant prefix, exactly like <c>tailwind-merge</c>:
/// <c>hover:bg-primary</c> never conflicts with <c>bg-secondary</c>, and <c>p-4</c> removes an
/// earlier <c>px-2</c> (a shorthand beats its longhands). A class the table does not know is
/// kept untouched — an unknown utility can never be dropped.
/// </remarks>
public static class TwMerge
{
    /// <summary>Merges a whitespace-separated class list; later classes win.</summary>
    public static string? Merge(string? classes)
    {
        if (string.IsNullOrWhiteSpace(classes))
        {
            return null;
        }

        var kept = new List<string>(16);
        var indexByGroup = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var token in classes.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            var (variants, utility) = SplitVariants(token);
            var group = Classify(utility, out var conflictingGroups);
            var scope = variants.Length == 0 ? string.Empty : variants + "|";

            if (conflictingGroups is not null)
            {
                foreach (var conflicting in conflictingGroups)
                {
                    if (indexByGroup.Remove(scope + conflicting, out var index))
                    {
                        kept[index] = string.Empty;
                    }
                }
            }

            if (indexByGroup.TryGetValue(scope + group, out var previous))
            {
                kept[previous] = string.Empty;
            }

            indexByGroup[scope + group] = kept.Count;
            kept.Add(token);
        }

        return string.Join(' ', kept.Where(static value => value.Length > 0));
    }

    private static (string Variants, string Utility) SplitVariants(string token)
    {
        var depth = 0;
        for (var i = token.Length - 1; i >= 0; i--)
        {
            var c = token[i];
            if (c is ']' or ')')
            {
                depth++;
            }
            else if (c is '[' or '(')
            {
                depth--;
            }
            else if (c == ':' && depth == 0)
            {
                return (token[..i], token[(i + 1)..]);
            }
        }

        return (string.Empty, token);
    }

    private static bool HasPrefix(string value, string prefix) =>
        value.Equals(prefix, StringComparison.Ordinal) || value.StartsWith(prefix + "-", StringComparison.Ordinal);

    private static string Classify(string utility, out string[]? conflicts)
    {
        conflicts = null;

        var value = utility;
        if (value.EndsWith('!'))
        {
            value = value[..^1];
        }

        if (value.StartsWith('-'))
        {
            value = value[1..];
        }

        // ---- exact utilities: display, position, visibility -------------------------------
        switch (value)
        {
            case "flex":
            case "grid":
            case "block":
            case "inline":
            case "inline-block":
            case "inline-flex":
            case "inline-grid":
            case "inline-table":
            case "table":
            case "table-row":
            case "table-cell":
            case "table-caption":
            case "flow-root":
            case "contents":
            case "hidden":
                return "display";
            case "static":
            case "fixed":
            case "absolute":
            case "relative":
            case "sticky":
                return "position";
            case "visible":
            case "invisible":
            case "collapse":
                return "visibility";
            case "sr-only":
            case "not-sr-only":
                return "sr";
            case "truncate":
                return "text-overflow";
            case "container":
                return "container";
            case "antialiased":
            case "subpixel-antialiased":
                return "font-smoothing";
            case "transform":
            case "transform-cpu":
            case "transform-gpu":
            case "transform-none":
                return "transform";
            case "filter":
            case "filter-none":
                return "filter";
            case "border":
                conflicts = ["border-w-x", "border-w-y", "border-w-t", "border-w-r", "border-w-b", "border-w-l", "border-w-s", "border-w-e"];
                return "border-width";
        }

        // ---- border ------------------------------------------------------------------------
        if (HasPrefix(value, "border"))
        {
            var rest = value["border-".Length..];
            return ClassifyBorder(rest, out conflicts);
        }

        // ---- divide ------------------------------------------------------------------------
        if (HasPrefix(value, "divide"))
        {
            var rest = value["divide-".Length..];
            if (rest is "solid" or "dashed" or "dotted" or "double" or "none")
            {
                return "divide-style";
            }

            if (rest is "x" or "y" || rest.StartsWith('x') || rest.StartsWith('y') || int.TryParse(rest, out _))
            {
                return rest.StartsWith('x') ? "divide-x" : rest.StartsWith('y') ? "divide-y" : "divide-width";
            }

            return "divide-color";
        }

        // ---- text --------------------------------------------------------------------------
        if (HasPrefix(value, "text"))
        {
            var rest = value["text-".Length..];
            return rest switch
            {
                "xs" or "sm" or "base" or "lg" or "xl" or "2xl" or "3xl" or "4xl" or "5xl" or "6xl" or "7xl" or "8xl" or "9xl" => "text-size",
                "left" or "center" or "right" or "justify" or "start" or "end" => "text-align",
                "wrap" or "nowrap" or "balance" or "pretty" => "text-wrap",
                "ellipsis" or "clip" => "text-overflow",
                _ when rest.StartsWith('[') && !rest.Contains("color", StringComparison.Ordinal) => "text-size",
                _ => "text-color",
            };
        }

        // ---- font / leading / tracking -------------------------------------------------------
        if (HasPrefix(value, "font"))
        {
            var rest = value["font-".Length..];
            return rest is "thin" or "extralight" or "light" or "normal" or "medium" or "semibold" or "bold" or "extrabold" or "black"
                ? "font-weight"
                : "font-family";
        }

        if (HasPrefix(value, "leading"))
        {
            return "leading";
        }

        if (HasPrefix(value, "tracking"))
        {
            return "tracking";
        }

        // ---- backgrounds ---------------------------------------------------------------------
        if (HasPrefix(value, "bg"))
        {
            var rest = value["bg-".Length..];
            if (rest is "fixed" or "local" or "scroll")
            {
                return "bg-attachment";
            }

            if (rest is "cover" or "contain" or "auto")
            {
                return "bg-size";
            }

            if (rest is "bottom" or "center" or "left" or "right" or "top" or "left-bottom" or "left-top" or "right-bottom" or "right-top")
            {
                return "bg-position";
            }

            if (rest.StartsWith("no-repeat", StringComparison.Ordinal) || rest is "repeat" or "repeat-x" or "repeat-y" or "repeat-round" or "repeat-space")
            {
                return "bg-repeat";
            }

            if (rest is "none" || rest.StartsWith("gradient-", StringComparison.Ordinal))
            {
                return "bg-image";
            }

            if (rest.StartsWith("clip-", StringComparison.Ordinal))
            {
                return "bg-clip";
            }

            if (rest.StartsWith("origin-", StringComparison.Ordinal))
            {
                return "bg-origin";
            }

            if (rest.StartsWith("blend-", StringComparison.Ordinal))
            {
                return "bg-blend";
            }

            if (rest.StartsWith("[url", StringComparison.Ordinal) || rest.StartsWith("image-", StringComparison.Ordinal))
            {
                return "bg-image";
            }

            if (rest.StartsWith("[length", StringComparison.Ordinal) || rest.StartsWith("[size", StringComparison.Ordinal))
            {
                return "bg-size";
            }

            if (rest.StartsWith("[position", StringComparison.Ordinal))
            {
                return "bg-position";
            }

            return "bg-color";
        }

        // ---- rounding ----------------------------------------------------------------------
        if (value == "rounded")
        {
            conflicts = ["rounded-t", "rounded-r", "rounded-b", "rounded-l", "rounded-tl", "rounded-tr", "rounded-br", "rounded-bl", "rounded-s", "rounded-e", "rounded-ss", "rounded-se", "rounded-es", "rounded-ee"];
            return "rounded";
        }

        if (HasPrefix(value, "rounded"))
        {
            var rest = value["rounded-".Length..];
            if (rest is "t" or "r" or "b" or "l" or "tl" or "tr" or "br" or "bl" or "s" or "e" or "ss" or "se" or "es" or "ee")
            {
                return "rounded-" + rest;
            }

            return "rounded";
        }

        // ---- elevation, focus rings, outline ------------------------------------------------
        if (HasPrefix(value, "shadow"))
        {
            return "shadow";
        }

        if (HasPrefix(value, "ring-offset"))
        {
            var rest = value["ring-offset-".Length..];
            return int.TryParse(rest, out _) || rest.StartsWith('[') && !rest.Contains("color", StringComparison.Ordinal)
                ? "ring-offset-width"
                : "ring-offset-color";
        }

        if (HasPrefix(value, "ring"))
        {
            var rest = value["ring-".Length..];
            if (rest is "inset" or "inset-x" or "inset-y" or "0" or "1" or "2" or "4" or "8")
            {
                return "ring-width";
            }

            if (int.TryParse(rest, out _))
            {
                return "ring-width";
            }

            if (rest.StartsWith('[') && !rest.Contains("color", StringComparison.Ordinal))
            {
                return "ring-width";
            }

            return "ring-color";
        }

        if (HasPrefix(value, "outline"))
        {
            var rest = value["outline-".Length..];
            if (rest is "none" or "hidden" or "dashed" or "dotted" or "double" or "solid")
            {
                return "outline-style";
            }

            if (int.TryParse(rest, out _) || rest.StartsWith('[') && !rest.Contains("color", StringComparison.Ordinal))
            {
                return "outline-width";
            }

            return "outline-color";
        }

        // ---- layout -------------------------------------------------------------------------
        if (HasPrefix(value, "inset"))
        {
            return "inset-" + value["inset-".Length..].Split('-')[0];
        }

        if (HasPrefix(value, "translate"))
        {
            var rest = value["translate-".Length..];
            return rest.StartsWith('x') ? "translate-x" : rest.StartsWith('y') ? "translate-y" : "translate";
        }

        if (HasPrefix(value, "scale"))
        {
            var rest = value["scale-".Length..];
            return rest.StartsWith('x') ? "scale-x" : rest.StartsWith('y') ? "scale-y" : "scale";
        }

        if (HasPrefix(value, "skew"))
        {
            var rest = value["skew-".Length..];
            return rest.StartsWith('x') ? "skew-x" : rest.StartsWith('y') ? "skew-y" : "skew";
        }

        if (HasPrefix(value, "flex"))
        {
            var rest = value["flex-".Length..];
            return rest is "row" or "row-reverse" or "col" or "col-reverse"
                ? "flex-direction"
                : rest is "wrap" or "wrap-reverse" or "nowrap"
                    ? "flex-wrap"
                    : "flex";
        }

        if (HasPrefix(value, "grow"))
        {
            return "grow";
        }

        if (HasPrefix(value, "shrink"))
        {
            return "shrink";
        }

        if (HasPrefix(value, "grid-cols"))
        {
            return "grid-cols";
        }

        if (HasPrefix(value, "grid-rows"))
        {
            return "grid-rows";
        }

        if (HasPrefix(value, "grid-flow"))
        {
            return "grid-flow";
        }

        if (HasPrefix(value, "auto-cols"))
        {
            return "auto-cols";
        }

        if (HasPrefix(value, "auto-rows"))
        {
            return "auto-rows";
        }

        if (HasPrefix(value, "col"))
        {
            return value.StartsWith("col-start", StringComparison.Ordinal) ? "col-start"
                : value.StartsWith("col-end", StringComparison.Ordinal) ? "col-end"
                : "col";
        }

        if (HasPrefix(value, "row"))
        {
            return value.StartsWith("row-start", StringComparison.Ordinal) ? "row-start"
                : value.StartsWith("row-end", StringComparison.Ordinal) ? "row-end"
                : "row";
        }

        if (HasPrefix(value, "gap"))
        {
            var rest = value["gap-".Length..];
            if (rest.StartsWith('x'))
            {
                return "gap-x";
            }

            if (rest.StartsWith('y'))
            {
                return "gap-y";
            }

            conflicts = ["gap-x", "gap-y"];
            return "gap";
        }

        if (HasPrefix(value, "space-x"))
        {
            return "space-x";
        }

        if (HasPrefix(value, "space-y"))
        {
            return "space-y";
        }

        if (HasPrefix(value, "items"))
        {
            return "items";
        }

        if (HasPrefix(value, "justify-items"))
        {
            return "justify-items";
        }

        if (HasPrefix(value, "justify-self"))
        {
            return "justify-self";
        }

        if (HasPrefix(value, "justify"))
        {
            return "justify";
        }

        if (HasPrefix(value, "place-items"))
        {
            return "place-items";
        }

        if (HasPrefix(value, "place-content"))
        {
            return "place-content";
        }

        if (HasPrefix(value, "place-self"))
        {
            return "place-self";
        }

        if (HasPrefix(value, "content"))
        {
            return "content";
        }

        if (HasPrefix(value, "self"))
        {
            return "self";
        }

        if (HasPrefix(value, "order"))
        {
            return "order";
        }

        if (HasPrefix(value, "overflow"))
        {
            return value.StartsWith("overflow-x", StringComparison.Ordinal) ? "overflow-x"
                : value.StartsWith("overflow-y", StringComparison.Ordinal) ? "overflow-y"
                : "overflow";
        }

        if (HasPrefix(value, "overscroll"))
        {
            return "overscroll";
        }

        if (HasPrefix(value, "object"))
        {
            var rest = value["object-".Length..];
            return rest is "contain" or "cover" or "fill" or "none" or "scale-down" ? "object-fit" : "object-position";
        }

        if (HasPrefix(value, "whitespace"))
        {
            return "whitespace";
        }

        if (HasPrefix(value, "break"))
        {
            return value.StartsWith("break-inside", StringComparison.Ordinal) ? "break-inside"
                : value.StartsWith("break-before", StringComparison.Ordinal) ? "break-before"
                : value.StartsWith("break-after", StringComparison.Ordinal) ? "break-after"
                : "word-break";
        }

        if (HasPrefix(value, "align"))
        {
            return "vertical-align";
        }

        if (HasPrefix(value, "table"))
        {
            return "table-layout";
        }

        if (HasPrefix(value, "line-clamp"))
        {
            return "line-clamp";
        }

        if (HasPrefix(value, "aspect"))
        {
            return "aspect";
        }

        if (HasPrefix(value, "cursor"))
        {
            return "cursor";
        }

        if (HasPrefix(value, "select"))
        {
            return "user-select";
        }

        if (HasPrefix(value, "pointer-events"))
        {
            return "pointer-events";
        }

        if (HasPrefix(value, "resize"))
        {
            return "resize";
        }

        if (HasPrefix(value, "appearance"))
        {
            return "appearance";
        }

        if (HasPrefix(value, "touch"))
        {
            return "touch";
        }

        if (HasPrefix(value, "will-change"))
        {
            return "will-change";
        }

        if (HasPrefix(value, "fill"))
        {
            return "fill";
        }

        if (HasPrefix(value, "stroke"))
        {
            var rest = value["stroke-".Length..];
            return int.TryParse(rest, out _) ? "stroke-width" : "stroke";
        }

        if (HasPrefix(value, "list"))
        {
            var rest = value["list-".Length..];
            return rest is "none" or "disc" or "decimal" ? "list-style" : "list-position";
        }

        if (HasPrefix(value, "animate"))
        {
            return "animate";
        }

        if (HasPrefix(value, "transition"))
        {
            return "transition";
        }

        if (HasPrefix(value, "duration"))
        {
            return "duration";
        }

        if (HasPrefix(value, "delay"))
        {
            return "delay";
        }

        if (HasPrefix(value, "ease"))
        {
            return "ease";
        }

        if (HasPrefix(value, "opacity"))
        {
            return "opacity";
        }

        if (HasPrefix(value, "z"))
        {
            return "z-index";
        }

        if (HasPrefix(value, "origin"))
        {
            return "transform-origin";
        }

        if (HasPrefix(value, "rotate"))
        {
            return "rotate";
        }

        if (HasPrefix(value, "columns"))
        {
            return "columns";
        }

        if (HasPrefix(value, "float"))
        {
            return "float";
        }

        if (HasPrefix(value, "clear"))
        {
            return "clear";
        }

        if (HasPrefix(value, "top") || HasPrefix(value, "right") || HasPrefix(value, "bottom")
            || HasPrefix(value, "left") || HasPrefix(value, "start") || HasPrefix(value, "end"))
        {
            return "position-" + value.Split('-')[0];
        }

        // ---- spacing scale -----------------------------------------------------------------
        if (value is "p" or "m"
            || (value.Length > 2 && value[0] is 'p' or 'm' && value[1] == '-'))
        {
            conflicts = value[0] == 'p'
                ? ["px", "py", "pt", "pr", "pb", "pl", "ps", "pe"]
                : ["mx", "my", "mt", "mr", "mb", "ml", "ms", "me"];
            return value[0] == 'p' ? "p" : "m";
        }

        if (value.Length >= 2
            && value[0] is 'p' or 'm'
            && value[1] is 'x' or 'y' or 't' or 'r' or 'b' or 'l' or 's' or 'e'
            && (value.Length == 2 || value[2] is '-' or '['))
        {
            var kind = value[0] == 'p' ? "p" : "m";
            var side = value[1].ToString();
            conflicts = (kind, value[1]) switch
            {
                ("p", 'x') => ["pl", "pr", "ps", "pe"],
                ("p", 'y') => ["pt", "pb"],
                ("m", 'x') => ["ml", "mr", "ms", "me"],
                ("m", 'y') => ["mt", "mb"],
                _ => null,
            };

            return kind + side;
        }

        if (HasPrefix(value, "w"))
        {
            return "w";
        }

        if (HasPrefix(value, "h"))
        {
            return "h";
        }

        if (HasPrefix(value, "size"))
        {
            return "size";
        }

        if (HasPrefix(value, "min-w"))
        {
            return "min-w";
        }

        if (HasPrefix(value, "min-h"))
        {
            return "min-h";
        }

        if (HasPrefix(value, "max-w"))
        {
            return "max-w";
        }

        if (HasPrefix(value, "max-h"))
        {
            return "max-h";
        }

        if (HasPrefix(value, "basis"))
        {
            return "basis";
        }

        if (HasPrefix(value, "sr"))
        {
            return "sr";
        }

        return "u:" + value;
    }

    private static string ClassifySpacing(string rest, string kind, out string[]? conflicts)
    {
        conflicts = null;
        if (rest.Length == 0)
        {
            conflicts = kind == "p"
                ? ["px", "py", "pt", "pr", "pb", "pl", "ps", "pe"]
                : ["mx", "my", "mt", "mr", "mb", "ml", "ms", "me"];
            return kind;
        }

        var side = rest[0];
        switch (side)
        {
            case 'x':
                conflicts = kind == "p" ? ["pl", "pr", "ps", "pe"] : ["ml", "mr", "ms", "me"];
                return kind + "x";
            case 'y':
                conflicts = kind == "p" ? ["pt", "pb"] : ["mt", "mb"];
                return kind + "y";
            case 't' or 'r' or 'b' or 'l' or 's' or 'e':
                if (rest.Length == 1 || char.IsDigit(rest[1]) || rest[1] is '[' or '-')
                {
                    return kind + side;
                }

                break;
        }

        return kind;
    }

    private static string ClassifyBorder(string rest, out string[]? conflicts)
    {
        conflicts = null;

        if (rest.Length == 0)
        {
            conflicts = ["border-w-x", "border-w-y", "border-w-t", "border-w-r", "border-w-b", "border-w-l", "border-w-s", "border-w-e"];
            return "border-width";
        }

        if (int.TryParse(rest, out _) || rest.StartsWith('[') && !rest.Contains("color", StringComparison.Ordinal))
        {
            conflicts = ["border-w-x", "border-w-y", "border-w-t", "border-w-r", "border-w-b", "border-w-l", "border-w-s", "border-w-e"];
            return "border-width";
        }

        if (rest is "solid" or "dashed" or "dotted" or "double" or "hidden" or "none")
        {
            return "border-style";
        }

        var side = rest[0];
        if (side is 'x' or 'y' or 't' or 'r' or 'b' or 'l' or 's' or 'e')
        {
            if (rest.Length == 1 || char.IsDigit(rest[1]) || rest[1] == '[')
            {
                return "border-w-" + side;
            }
        }

        return "border-color";
    }
}
