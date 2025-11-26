# Elvator plugin
Its a (elevator plugin)

# About
> A ui pops up with how many floors u configure
> you click the floor you wanna go to and it will tp you there

```xml
<?xml version="1.0" encoding="utf-8"?>
<ElevatorPluginConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <UIEffectID>22008</UIEffectID>
  <CloseButtonName>Elevator_Close</CloseButtonName>
  <Elevators>
    <Elevator Name="MainLobbyElevator">
      <Position>
        <X>-239.04</X>
        <Y>44</Y>
        <Z>23.73</Z>
      </Position>
      <Radius>5</Radius>
      <UseZoneTrigger>true</UseZoneTrigger>
      <TriggerItemID>0</TriggerItemID>
      <Floors>
        <Floor>
          <ButtonName>Floor_1</ButtonName>
          <DisplayName>Lobby</DisplayName>
          <Destination>
            <X>100</X>
            <Y>50</Y>
            <Z>-20</Z>
          </Destination>
        </Floor>
        <Floor>
          <ButtonName>Floor_2</ButtonName>
          <DisplayName>Apartments</DisplayName>
          <Destination>
            <X>100</X>
            <Y>70</Y>
            <Z>-20</Z>
          </Destination>
        </Floor>
        <Floor>
          <ButtonName>Floor_3</ButtonName>
          <DisplayName>Rooftop</DisplayName>
          <Destination>
            <X>100</X>
            <Y>90</Y>
            <Z>-20</Z>
          </Destination>
        </Floor>
      </Floors>
    </Elevator>
    <Elevator Name="SecretBunkerElevator">
      <Position>
        <X>-45</X>
        <Y>20</Y>
        <Z>150</Z>
      </Position>
      <Radius>2</Radius>
      <UseZoneTrigger>false</UseZoneTrigger>
      <TriggerItemID>328</TriggerItemID>
      <Floors>
        <Floor>
          <ButtonName>Floor_1</ButtonName>
          <DisplayName>Surface</DisplayName>
          <Destination>
            <X>-45</X>
            <Y>20</Y>
            <Z>150</Z>
          </Destination>
        </Floor>
        <Floor>
          <ButtonName>Floor_2</ButtonName>
          <DisplayName>Bunker Level 1</DisplayName>
          <Destination>
            <X>-45</X>
            <Y>-10</Y>
            <Z>150</Z>
          </Destination>
        </Floor>
      </Floors>
    </Elevator>
  </Elevators>
</ElevatorPluginConfiguration>
```
> im going to imrpove this plugin in the future with loading screen effects for floors etc
# Unturned Store Form plugin
