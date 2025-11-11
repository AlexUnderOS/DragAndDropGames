# DragAndDropGames
_Šī spēle ir paredzēta bērniem no 6 gadu vecuma. Spēle veicina bērna elastīgas domāšanas attīstību, kas viņam/viņai nākotnē dos pozitīvu ietekmi. Spēles galvenā ideja ir apzināta objektu pārvietošana_

**To do list:**
- [x] Create the necessary folders 
- [x] Add necessary assets 
- [x] Add cars on the map
- [x] Create C# script for drag and drop
- [x] Create C# script for transformation
- [x] Create C# script for object fixation
- [x] Add necessary sounds and audio sources
- [X] Create logic for winning
- [x] Create camera script for zoom-in/out and camera restrictions
- [X] Create animated main menu with 3 buttons, sounds, animated objects
- [X] Create C# script for scene change and quit option
- [X] Create game timer (HH:MM:SS)
- [X] Add animated clouds, vehicles, people, animals etc.
- [x] Add flying obstacles with destroy effects
- [ ] Replace all mouse input with touch
- [X] Fix camera max zoom
- [X] Add interstitial ad
- [ ] Add rewarded ad
- [ ] Add banner ad

## **Līmenis 1**
<img width="1280" height="718" alt="8" src="https://github.com/user-attachments/assets/3477d6aa-a785-4bc5-b028-ef4a7016aa19" />

- Lai sāktu spēlēt, nospiediet 1. līmeni.

<pre>
pirmais līmenis
       |
       |
      \|/ </pre>

<img width="1280" height="719" alt="1" src="https://github.com/user-attachments/assets/41874788-e20a-4abf-ab3a-50c7d2d6ac1d" />
<hr>

### Instrukcija, kā uzvarēt:
- Jānovieto automašīnas savās vietās
- Spēle neļaus jums novietot automašīnu neatbilstošā vietā

<img width="748" height="479" alt="2" src="https://github.com/user-attachments/assets/868f2029-a37c-422b-94da-51e7dc0c6fb5" />
<img width="315" height="279" alt="4" src="https://github.com/user-attachments/assets/cbe5b246-81f6-45b5-ad8d-77da8685b565" />

- Jāņem vērā, ka mašīnas parasti nav savā normālajā izmērā/rotācijā, tāpēc tās ir jāpielāgo konkrētās mašīnas izmēriem.
  - Taustiņi z/x - pagriež mašīnu pa kreisi/pa labi
  - Bultiņas: ←, →, ↑, ↓ maina izmēru
- Izmērs un rotācija var nebūt ideāli, bet tiem jābūt līdzīgiem, aptuveni šādiem:

<img width="510" height="373" alt="3" src="https://github.com/user-attachments/assets/f3baa809-147b-4982-b7c3-e26b93438a19" />
<hr>

### Palīgelementi (bumbas)

Kā izskatās bumba:
<img width="167" height="144" alt="6" src="https://github.com/user-attachments/assets/8ddcfaf7-8701-4dbe-8d8c-38d5de7ff2cf" />

- Nospiežot uz bumbu, tā eksplodē, tādējādi iznīcinot blakus esošos objektus:
<img width="348" height="254" alt="5" src="https://github.com/user-attachments/assets/b3c6b882-cc4b-4944-aa00-4f0d7c529202" />
<hr>

### Rezultātu ekrāns
<img width="1280" height="719" alt="7" src="https://github.com/user-attachments/assets/01a2d9a2-591c-4745-a997-8d72af0d2bb2" />

1. Laiks, kurā mēs izgājām līmeni
2. Top 5 labākie rezultāti (saglabājas pēc pārstartēšanas)
3. Atkarībā no tā, cik ātri tu izieti līmeni, tev piešķirs zvaigznes, jo vairāk zvaigžņu, jo labāks rezultāts (0 zvaigznes nozīmē, ka pārāk ilgi ilga)
4. Poga, kas mūs atgriež atpakaļ izvēlnē
5. poga, kas pārstartē līmeni
<hr>
