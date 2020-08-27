# **TODO**

## Design

- design "edit classes" window
	- can add, remove or edit
	- need input for every attribute of a class
- design "setting" window
- design "about" window
- design "information" window on event from dataGrid
- add information section on main window

## Code

- Have to seperate setting from the classes xml, need to rewrite some code from the library for explicit setting uses. the console version will need to be adjusted
	- setting.xml and schoolClasses.xml
- "Are you sure" on send to google.
- Make the progress bar work.
- Selecting a class on the left panel reveal informations
- DoubleClicking an event in the dataGrid open an information window.
- settings.xml should look like this
	```XML
	<settings>
		<setting
			year="2020"
			timeZone="America/Toronto"
			imageWidth="1528"
			imageHeight="1726"
			codeMatch="[0-9a-zA-Z]{3}[-_—][0-9a-zA-Z]{3}[-_—]R[O00]"
			monthsMatch="janv, f[eé]vr, mars, avr, ma[li], juin, jui[li], ao.t, sept, oct, nov, d[ée]c"/>
		<defaultClass
			group="392"
			colorId="10"/>
	<settings/>
	```


## Other

- pls commit, it's been a while now.