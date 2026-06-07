using Godot;
using HadesAncients.HadesAncientsCode.Shared.Enums;

namespace HadesAncients.HadesAncientsCode.Shared.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path, HadesAncient hadesAncient)
    {
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", path);
    }

    public static string CardImagePath(this string path, HadesAncient hadesAncient)
    {
        path = Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "card_portraits", path);
        if (ResourceLoader.Exists(path)) return path;

        HadesAncientsMainFile.Logger.Info("Could not find card image path: " + path);
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "card_portraits",
            "card.png");
    }

    public static string BigCardImagePath(this string path, HadesAncient hadesAncient)
    {
        path = Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "card_portraits", "big",
            path);
        if (ResourceLoader.Exists(path)) return path;

        HadesAncientsMainFile.Logger.Info("Could not find big card image path: " + path);
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "card_portraits", "big",
            "card.png");
    }

    public static string PowerImagePath(this string path, HadesAncient hadesAncient)
    {
        path = Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "powers", path);
        if (ResourceLoader.Exists(path)) return path;

        HadesAncientsMainFile.Logger.Info("Could not find power image path: " + path);
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "powers", "power.png");
    }

    public static string BigPowerImagePath(this string path, HadesAncient hadesAncient)
    {
        path = Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "powers", "big", path);
        if (ResourceLoader.Exists(path)) return path;

        HadesAncientsMainFile.Logger.Info("Could not find big power image path: " + path);
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "powers", "big",
            "power.png");
    }

    public static string RelicImagePath(this string path, HadesAncient hadesAncient)
    {
        path = Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "relics", path);
        if (ResourceLoader.Exists(path)) return path;

        HadesAncientsMainFile.Logger.Info("Could not find relic image path: " + path);
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "relics", "relic.png");
    }

    public static string RelicOutlineImagePath(this string path, HadesAncient hadesAncient)
    {
        path = Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "relics", "outline", path);
        if (ResourceLoader.Exists(path)) return path;

        HadesAncientsMainFile.Logger.Info("Could not find relic image path: " + path);
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "relics", "outline",
            "relic.png");
    }

    public static string BigRelicImagePath(this string path, HadesAncient hadesAncient)
    {
        path = Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "relics", "big", path);
        if (ResourceLoader.Exists(path)) return path;

        HadesAncientsMainFile.Logger.Info("Could not find big relic image path: " + path);
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "relics", "big",
            "relic.png");
    }

    public static string CharacterUiPath(this string path, HadesAncient hadesAncient)
    {
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "charui", path);
    }

    public static string SoundPath(this string path, HadesAncient hadesAncient)
    {
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "sounds", path);
    }

    public static string AncientImagePath(this string path, HadesAncient hadesAncient)
    {
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "ancients", path);
    }

    public static string EnchantmentImagePath(this string path, HadesAncient hadesAncient)
    {
        return Path.Join(HadesAncientsMainFile.ResPath, hadesAncient.ToString(), "images", "enchantments", path);
    }
}