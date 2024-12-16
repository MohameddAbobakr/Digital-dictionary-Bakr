open System
open System.IO
open System.Windows.Forms
open Newtonsoft.Json

// Define the dictionary class
type DigitalDictionaryBakr() =
    let mutable dictionary: Map<string, string> = Map.empty

    // Add a word
    member this.AddWord(word: string, definition: string) =
        if dictionary.ContainsKey(word.ToLower()) then
            "Word already exists. Use UpdateWord to change the definition."
        else
            dictionary <- dictionary.Add(word.ToLower(), definition)
            "Word added successfully."

    // Update a word's definition
    member this.UpdateWord(word: string, newDefinition: string) =
        if dictionary.ContainsKey(word.ToLower()) then
            dictionary <- dictionary.Add(word.ToLower(), newDefinition)
            "Word updated successfully."
        else
            "Word not found."

    // Delete a word
    member this.DeleteWord(word: string) =
        if dictionary.ContainsKey(word.ToLower()) then
            dictionary <- dictionary.Remove(word.ToLower())
            "Word deleted successfully."
        else
            "Word not found."

    // Search for a word
    member this.SearchWord(word: string) =
        match dictionary.TryFind(word.ToLower()) with
        | Some definition -> sprintf "Definition: %s" definition
        | None -> "Word not found."

    // Search for partial matches
    member this.SearchPartial(keyword: string) =
        let results = 
            dictionary 
            |> Map.filter (fun key _ -> key.Contains(keyword.ToLower()))
        if results.IsEmpty then
            "No matches found."
        else
            results |> Map.fold (fun acc key value -> acc + sprintf "%s: %s\n" key value) ""

    // Save dictionary to a file
    member this.SaveToFile(filePath: string) =
        let json = JsonConvert.SerializeObject(dictionary)
        File.WriteAllText(filePath, json)
        "Dictionary saved successfully."

    // Load dictionary from a file
    member this.LoadFromFile(filePath: string) =
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            dictionary <- JsonConvert.DeserializeObject<Map<string, string>>(json)
            "Dictionary loaded successfully."
        else
            "File not found."

// Main function to build the UI
[<STAThread>] // Indicates the application uses single-threaded apartment (STA) model
[<EntryPoint>] // Specifies the program entry point
let main argv =
    let dictionary = new DigitalDictionaryBakr()

    // Create the form
    let form = new Form(Text = "Digital Dictionary Bakr", Width = 600, Height = 500)

    // Create controls
    let lblWord = new Label(Text = "Word:", Top = 20, Left = 20, Width = 50)
    let txtWord = new TextBox(Top = 20, Left = 100, Width = 300)

    let lblDefinition = new Label(Text = "Definition:", Top = 60, Left = 20, Width = 70)
    let txtDefinition = new TextBox(Top = 60, Left = 100, Width = 300)

    let btnAdd = new Button(Text = "Add", Top = 100, Left = 20, Width = 80)
    let btnUpdate = new Button(Text = "Update", Top = 100, Left = 120, Width = 80)
    let btnDelete = new Button(Text = "Delete", Top = 100, Left = 220, Width = 80)
    let btnSearch = new Button(Text = "Search", Top = 100, Left = 320, Width = 80)

    let lblSearchResults = new Label(Text = "Search Results:", Top = 150, Left = 20, Width = 150)
    let txtSearchResults = new TextBox(Top = 180, Left = 20, Width = 550, Height = 200, Multiline = true, ReadOnly = true)

    let btnSave = new Button(Text = "Save", Top = 400, Left = 20, Width = 80)
    let btnLoad = new Button(Text = "Load", Top = 400, Left = 120, Width = 80)

    // Event handlers
    btnAdd.Click.Add(fun _ ->
        let message = dictionary.AddWord(txtWord.Text, txtDefinition.Text)
        MessageBox.Show(message) |> ignore
        txtWord.Clear()
        txtDefinition.Clear()
    )

    btnUpdate.Click.Add(fun _ ->
        let message = dictionary.UpdateWord(txtWord.Text, txtDefinition.Text)
        MessageBox.Show(message) |> ignore
        txtWord.Clear()
        txtDefinition.Clear()
    )

    btnDelete.Click.Add(fun _ ->
        let message = dictionary.DeleteWord(txtWord.Text)
        MessageBox.Show(message) |> ignore
        txtWord.Clear()
    )

    btnSearch.Click.Add(fun _ ->
        let results = 
            if txtWord.Text.Contains(" ") then dictionary.SearchWord(txtWord.Text)
            else dictionary.SearchPartial(txtWord.Text)
        txtSearchResults.Text <- results
    )

    btnSave.Click.Add(fun _ ->
        let message = dictionary.SaveToFile("dictionary.json")
        MessageBox.Show(message) |> ignore
    )

    btnLoad.Click.Add(fun _ ->
        let message = dictionary.LoadFromFile("dictionary.json")
        MessageBox.Show(message) |> ignore
    )

    // Add controls to the form
    form.Controls.AddRange([| lblWord; txtWord; lblDefinition; txtDefinition
                              btnAdd; btnUpdate; btnDelete; btnSearch
                              lblSearchResults; txtSearchResults
                              btnSave; btnLoad |])

    // Run the application
    Application.Run(form)

    0