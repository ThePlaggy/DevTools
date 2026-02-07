using System.Collections.Generic;
using UnityEngine;

public class NewTenantGenerator
{
	private int dynamicID;

	public List<string> FirstManNames;

	public List<string> FirstWomanNames;

	public List<string> LastNames;

	public List<NewTenantNoteDictionary> NotesCT;

	public List<NewTenantNoteDictionary> NotesF;

	public List<NewTenantNoteDictionary> NotesM;

	public NewTenantGenerator()
	{
		FirstManNames = new List<string>();
		FirstWomanNames = new List<string>();
		LastNames = new List<string>();
		NotesM = new List<NewTenantNoteDictionary>();
		NotesF = new List<NewTenantNoteDictionary>();
		NotesCT = new List<NewTenantNoteDictionary>();
		dynamicID = 198000;
		FirstManNames = new List<string>
		{
			"Alden", "Alex", "Alfred", "Alonso", "Adam", "Anthony", "Benjamin", "Billy", "Brayden", "Brendan",
			"Brian", "Carlos", "Carlos", "Chad", "Cristian", "Cristopher", "Dale", "Damon", "Daniel", "Darren",
			"David", "Dawson", "Deacon", "Denise", "Donald", "Drake", "Edwards", "Erik", "Ethan", "Fletcher",
			"Gary", "George", "Gianni", "Hudson", "Jack", "James", "Jay", "John", "Jonathan", "Joseph",
			"Justin", "Keith", "Kevin", "Kyle", "Mark", "Markus", "Mason", "Micah", "Michael", "Miles",
			"Nash", "Paul", "Patrik", "Peter", "Richard", "Ronald", "Ronnie", "Ryan", "Samuel", "Shawn",
			"Steven", "Thomas", "Tommy", "Tony", "Walter"
		};
		FirstWomanNames = new List<string>
		{
			"Alice", "April", "Ashlynn", "Ava", "Bria", "Carissa", "Clare", "Dana", "Delilah", "Diya",
			"Elizabeth", "Emily", "Emma", "Haylie", "Hillary", "Janiah", "Jasmine", "Jennifer", "Jenny", "Jessica",
			"Jordyn", "Judy", "Kellen", "Kylie", "Larissa", "Laura", "Lea", "Lilia", "Linda", "Lisa",
			"Londyn", "Lori", "Marie", "Margaret", "Miriam", "Nellie", "Nancy", "Noe", "Olivia", "Patricia",
			"Reese", "Rosemary", "Sandra", "Shea", "Skylar", "Sophia", "Stacey", "Susan"
		};
		LastNames = new List<string>
		{
			"Alexander", "Anderson", "Beasley", "Blake", "Blevins", "Bob", "Brown", "Carter", "Chambers", "Chen",
			"Clay", "Cooke", "Crosby", "Dalton", "Davenport", "Davis", "Downs", "Everett", "Ferguson", "Franklin",
			"Frost", "Gray", "Green", "Hogan", "Holden", "Holloway", "Jackson", "Johnson", "Jones", "Kelly",
			"Lozano", "Marquez", "Mata", "Miller", "Moon", "Morales", "Morris", "Nicholson", "Osborn", "Park",
			"Patel", "Pearson", "Pennington", "Pizza", "Rangel", "Raymond", "Roberts", "Robinson", "Rocha", "Ross",
			"Ryan", "Salazar", "Sanches", "Santiago", "Schwartz", "Shaw", "Smith", "Steele", "Sullivan", "Sutton",
			"Thomas", "Tucker", "Tyler", "Vincent", "Walters", "White", "Whitehead", "Williams", "Wills", "Wilson",
			"Woodward", "Wright"
		};
		NotesM = new List<NewTenantNoteDictionary>
		{
			new NewTenantNoteDictionary("Watches adult films very loudly.", 20, 45),
			new NewTenantNoteDictionary("Veteran", 52, 81),
			new NewTenantNoteDictionary("Reeks of cigarettes, avid chain smoker.", 18, 33),
			new NewTenantNoteDictionary("Divorced", 35, 51),
			new NewTenantNoteDictionary("Constant problems with police", 18, 40),
			new NewTenantNoteDictionary("Performs local volunteer work", 18, 37),
			new NewTenantNoteDictionary("Is clean", 48, 68),
			new NewTenantNoteDictionary("Has track marks", 18, 45),
			new NewTenantNoteDictionary("Creeps out female tenants", 40, 70),
			new NewTenantNoteDictionary("Argues with wife", 27, 49),
			new NewTenantNoteDictionary("You can hear the sounds of his mistress beating him into submission every Sunday and Thursday from 7pm-10pm. Would recommend ear plugs for those days.", 25, 60),
			new NewTenantNoteDictionary("In last 3 years he has a friendly beautiful wife that i ever met, but since his wife's death he never gooes outside.", 40, 65),
			new NewTenantNoteDictionary("All pays rent 1 day early, and deathly silent, nice man though.", 40, 70),
			new NewTenantNoteDictionary("Often has nieces vist him.", 19, 41),
			new NewTenantNoteDictionary("Always wears gloves", 18, 50),
			new NewTenantNoteDictionary("Usually late on rent", 18, 69),
			new NewTenantNoteDictionary("Works out a lot, alway takes early morning jobs.", 18, 44),
			new NewTenantNoteDictionary("Probably a meth addict.", 18, 22),
			new NewTenantNoteDictionary("Wants to move two flights up", 35, 44),
			new NewTenantNoteDictionary("Recently made redundant. May be late on payment this month.", 24, 54),
			new NewTenantNoteDictionary("For sure not getting their security deposit back", 26, 53),
			new NewTenantNoteDictionary("Keeps some interesting \"surprises\" underneath the bathroom floorboards.", 37, 59),
			new NewTenantNoteDictionary("Currently overseas", 18, 29),
			new NewTenantNoteDictionary("Always smells peculiar (formaldehyde?)", 30, 68),
			new NewTenantNoteDictionary("Has severe body odor.", 34, 68),
			new NewTenantNoteDictionary("Walks with a limp.", 25, 68),
			new NewTenantNoteDictionary("Works from home and has frequent video conference calls.", 19, 27),
			new NewTenantNoteDictionary("Has a large collection of musical instruments and plays them often.", 19, 39),
			new NewTenantNoteDictionary("Has a history of loud arguments with his partner", 18, 27),
			new NewTenantNoteDictionary("Fitness enthusiast, a true gym bro", 18, 31),
			new NewTenantNoteDictionary("Is a heavy smoker and often smokes indoors", 32, 70),
			new NewTenantNoteDictionary("Plays online games until late nights and sometimes is loud.", 18, 31),
			new NewTenantNoteDictionary("Is a college student, who often stays up late studying and making noise.", 18, 25),
			new NewTenantNoteDictionary("Enjoys playing loud music during the day.", 18, 36)
		};
		NotesF = new List<NewTenantNoteDictionary>
		{
			new NewTenantNoteDictionary("Carries her chihuahua in her purse.", 18, 34),
			new NewTenantNoteDictionary("Plumbing clogged again. Think she's doing it on purpose.", 22, 44),
			new NewTenantNoteDictionary("Works late nights and always pays rent with cash.", 21, 46),
			new NewTenantNoteDictionary("Is a flute player", 18, 36),
			new NewTenantNoteDictionary("Is a night owl.", 18, 29),
			new NewTenantNoteDictionary("Always orders take out, never leaves apartment.", 18, 31),
			new NewTenantNoteDictionary("Hair smells like lavender", 18, 24),
			new NewTenantNoteDictionary("Has a pet turtle, works as a nurse in the nearby hospital", 25, 40),
			new NewTenantNoteDictionary("Rarely comes out of apartment and is socially awkward.", 18, 37),
			new NewTenantNoteDictionary("Complaints from members staying at this apartment complex due to hearing strange noises. Final warning.", 26, 39),
			new NewTenantNoteDictionary("Has very LOUD dog. Barks a lot.", 31, 59),
			new NewTenantNoteDictionary("Way to noisy, cops called several times for disturbances.", 18, 31),
			new NewTenantNoteDictionary("Parents vist often, worried about her. Not sure about this one.", 18, 27),
			new NewTenantNoteDictionary("Clean freak, really werid. Leaves shoes outside door.", 32, 51),
			new NewTenantNoteDictionary("Always has shady people around.", 18, 26),
			new NewTenantNoteDictionary("Is a late payer... Likely an alcoholic apartment smells of strong liquor", 29, 59),
			new NewTenantNoteDictionary("Neglects house maintenance", 22, 48),
			new NewTenantNoteDictionary("Aspiring singer, plays piano all day and signs.", 18, 36),
			new NewTenantNoteDictionary("Often comes drunk at night. Has a lot of bruises.", 24, 46),
			new NewTenantNoteDictionary("Sleep walks", 18, 48),
			new NewTenantNoteDictionary("Has the consistency to talk everyone’s ear off.", 18, 40),
			new NewTenantNoteDictionary("Has loud parties on weekends.", 19, 38),
			new NewTenantNoteDictionary("Enjoys cooking exotic foods and the smell may travel to neighboring apartments.", 18, 41),
			new NewTenantNoteDictionary("Practices yoga at home and may cause noise disturbances.", 18, 26),
			new NewTenantNoteDictionary("Has a tendency to forget her keys and frequently needs to be let in by neighbors.", 18, 46),
			new NewTenantNoteDictionary("Has a pet parrot that can be loud and disruptive at times.", 28, 53),
			new NewTenantNoteDictionary("Has a habit of singing loudly in the shower, causing noise disturbances.", 18, 51),
			new NewTenantNoteDictionary("Works from home, often up late at night making noise.", 21, 39),
			new NewTenantNoteDictionary("Has a cat, which she is very protective of.", 20, 45),
			new NewTenantNoteDictionary("Has a lot of visitors, some of whom are making loud noises.", 18, 31),
			new NewTenantNoteDictionary("Is very particular about the cleaniness of common areas and often complains to management.", 36, 49),
			new NewTenantNoteDictionary("Social butteryfly, Has a lot of guests frequently", 18, 27),
			new NewTenantNoteDictionary("Very nice, always brings down cookies.", 48, 70),
			new NewTenantNoteDictionary("Typical college drunk.", 18, 25),
			new NewTenantNoteDictionary("Studying for PHD, always pays on time.", 18, 25)
		};
		NotesCT = new List<NewTenantNoteDictionary>
		{
			new NewTenantNoteDictionary("Is out on vacation for 3 months", 18, 40),
			new NewTenantNoteDictionary("Works night shifts.", 18, 30),
			new NewTenantNoteDictionary("Is currently out of town for family matters. Will be back at the end of the month. Rent payment expected to be made on schedule.", 18, 50),
			new NewTenantNoteDictionary("Havent seen in weeks. No response, mail is piling up.", 18, 40),
			new NewTenantNoteDictionary("Hangs out at night every day, sometimes coming as late as 5 AM", 18, 20),
			new NewTenantNoteDictionary("Doesn't live here anymore, eventhough the apartment's ownership is signed on her name. Her brother lives here instead.", 18, 30),
			new NewTenantNoteDictionary("She's been living with her boyfriend for a while now.", 18, 30),
			new NewTenantNoteDictionary("Her parents are in town, so she's temporarily moved out", 18, 40),
			new NewTenantNoteDictionary("She's taking some time off to travel and explore new places.", 18, 35),
			new NewTenantNoteDictionary("Currently undergoing medical treatment, and won't be back for a while.", 18, 45),
			new NewTenantNoteDictionary("House-sitting a friend for a while now, Won't be back soon.", 18, 40),
			new NewTenantNoteDictionary("Moved out at her sister's place until the end of the month.", 18, 30),
			new NewTenantNoteDictionary("Yesterday went camping with friends, won't be back for another week or two.", 18, 25),
			new NewTenantNoteDictionary("She is visiting family out of this state, Don't know when she will be back.", 18, 35),
			new NewTenantNoteDictionary("On a business trip.", 18, 30),
			new NewTenantNoteDictionary("Disappeared as of few days, Nobody has seen her around.", 18, 30),
			new NewTenantNoteDictionary("Haven't seen her in weeks, Zero noises from the apartment, Not sure if she's around.", 18, 30),
			new NewTenantNoteDictionary("She's studying abroad this semester, so she won't be back until next year.", 18, 25),
			new NewTenantNoteDictionary("She's on a cruise with her family for a few weeks now.", 18, 40),
			new NewTenantNoteDictionary("Not at home.", 18, 65)
		};
	}

	public TenantDefinition NewMaleTenant()
	{
		TenantDefinition tenantDefinition = ScriptableObject.CreateInstance<TenantDefinition>();
		int index = Random.Range(0, NotesM.Count);
		int index2 = Random.Range(0, FirstManNames.Count);
		int index3 = Random.Range(0, LastNames.Count);
		tenantDefinition.canBeTagged = false;
		tenantDefinition.tenantSex = SEX.MALE;
		tenantDefinition.tenantNotes = NotesM[index].note;
		tenantDefinition.tenantAge = Random.Range(NotesM[index].ageMin, NotesM[index].ageMax);
		tenantDefinition.tenantName = FirstManNames[index2] + " " + LastNames[index3];
		tenantDefinition.id = dynamicID;
		dynamicID++;
		NotesM.RemoveAt(index);
		FirstManNames.RemoveAt(index2);
		LastNames.RemoveAt(index3);
		return tenantDefinition;
	}

	public TenantDefinition NewFemaleTenant(bool active = true)
	{
		TenantDefinition tenantDefinition = ScriptableObject.CreateInstance<TenantDefinition>();
		int index = (active ? Random.Range(0, NotesF.Count) : Random.Range(0, NotesCT.Count));
		int index2 = Random.Range(0, FirstWomanNames.Count);
		int index3 = Random.Range(0, LastNames.Count);
		tenantDefinition.canBeTagged = active;
		tenantDefinition.tenantSex = SEX.FEMALE;
		tenantDefinition.tenantNotes = (active ? NotesF[index].note : NotesCT[index].note);
		tenantDefinition.tenantAge = Random.Range(active ? NotesF[index].ageMin : NotesCT[index].ageMin, active ? NotesF[index].ageMax : NotesCT[index].ageMax);
		tenantDefinition.tenantName = FirstWomanNames[index2] + " " + LastNames[index3];
		tenantDefinition.id = dynamicID;
		dynamicID++;
		if (active)
		{
			NotesF.RemoveAt(index);
		}
		else
		{
			NotesCT.RemoveAt(index);
		}
		FirstWomanNames.RemoveAt(index2);
		LastNames.RemoveAt(index3);
		return tenantDefinition;
	}
}
