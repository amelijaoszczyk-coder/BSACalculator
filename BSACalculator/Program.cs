// BSA & Clinical Dose Calculator
// Mosteller (BSA) · Devine (IBW) · Janmahasatian (ABW)
// Author: Amelia Oszczyk

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool runAgain = true;

while (runAgain)
{
    Console.Clear();

    string name   = Ask("Patient name or ID: ");
    double height = AskDouble("Height (cm): ");
    double weight = AskDouble("Weight (kg): ");
    int    age    = AskInt("Age (years): ");
    string sex    = AskSex("Sex (M/F): ");

    // BSA — Mosteller
    double bsa = Math.Sqrt((height * weight) / 3600.0);

    // BMI
    double h    = height / 100.0;
    double bmi  = weight / (h * h);

    // IBW — Devine
    double inches = height / 2.54;
    double ibw    = (sex == "M" ? 50.0 : 45.5) + 2.3 * (inches - 60.0);

    // ABW — Janmahasatian (only when actual weight > 130% IBW)
    bool   obese        = weight > 1.3 * ibw;
    double abw          = ibw + 0.4 * (weight - ibw);
    double dosingWeight = obese ? abw : weight;

    Console.WriteLine();
    Console.WriteLine($"Patient:   {name}, {age} yo, {sex}");
    Console.WriteLine($"BMI:       {bmi:F1} kg/m^2  ({BmiLabel(bmi)})");
    Console.WriteLine($"BSA:       {bsa:F2} m^2  (Mosteller)");
    Console.WriteLine($"IBW:       {ibw:F1} kg  (Devine)");
    if (obese)
        Console.WriteLine($"ABW:       {abw:F1} kg  (Janmahasatian)");
    Console.WriteLine($"Dosing wt: {dosingWeight:F1} kg  ({(obese ? "ABW used — weight >130% IBW" : "actual weight")})");

    // Optional dose
    Console.WriteLine();
    string drug = Ask("Drug name (Enter to skip): ");

    if (!string.IsNullOrEmpty(drug))
    {
        Console.WriteLine("1 = mg/m^2   2 = mg/kg");
        string method = Ask("Method: ");

        if (method == "1")
        {
            double dpm2  = AskDouble("Dose (mg/m^2): ");
            double total = dpm2 * bsa;
            Console.WriteLine();
            Console.WriteLine($"{drug}: {dpm2} mg/m^2 × BSA {bsa:F2} m² = {total:F1} mg");
        }
        else
        {
            double dpkg  = AskDouble("Dose (mg/kg): ");
            double total = dpkg * dosingWeight;
            Console.WriteLine();
            Console.WriteLine($"{drug}: {dpkg} mg/kg × {dosingWeight:F1} kg = {total:F1} mg");
        }
    }

    // Warnings
    Console.WriteLine();
    if (obese)        Console.WriteLine("Obese patient — verify dosing strategy.");
    if (age >= 65)    Console.WriteLine("Elderly patient — verify dosing strategy.");
    Console.WriteLine("Educational use only. Always verify with clinical guidelines.");

    Console.WriteLine();
    runAgain = Ask("Another patient? (yes/no): ").ToLower() is "yes" or "y";
}

// ── Helpers ───────────────────────────────────────────────────────────────────

static string Ask(string msg)
{
    Console.Write(msg);
    return Console.ReadLine()?.Trim() ?? "";
}

static double AskDouble(string msg)
{
    while (true)
    {
        Console.Write(msg);
        if (double.TryParse(Console.ReadLine()?.Trim(),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out double v) && v > 0) return v;
        Console.WriteLine("Please enter a positive number.");
    }
}

static int AskInt(string msg)
{
    while (true)
    {
        Console.Write(msg);
        if (int.TryParse(Console.ReadLine()?.Trim(), out int v) && v > 0) return v;
        Console.WriteLine("Please enter a whole number.");
    }
}

static string AskSex(string msg)
{
    while (true)
    {
        Console.Write(msg);
        string s = Console.ReadLine()?.Trim().ToUpper() ?? "";
        if (s == "M" || s == "F") return s;
        Console.WriteLine("Please enter M or F.");
    }
}

static string BmiLabel(double bmi) => bmi switch
{
    < 18.5 => "underweight",
    < 25.0 => "normal weight",
    < 30.0 => "overweight",
    < 35.0 => "obese class I",
    < 40.0 => "obese class II",
    _      => "obese class III"
};