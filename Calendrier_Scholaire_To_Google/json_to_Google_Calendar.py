#!/usr/bin/python3

# this script will take a json file corresponding to a list of events and
# add it to your google calendar

# references
# https://developers.google.com/calendar/quickstart/python



from __future__ import print_function
import datetime
import pickle
import json
import os.path
import sys
from googleapiclient.discovery import build
from google_auth_oauthlib.flow import InstalledAppFlow
from google.auth.transport.requests import Request


SCOPES = ['https://www.googleapis.com/auth/calendar']

def main():

        # setup for the Google API
    #####################################################################################################################################################################

    creds = None
    # The file token.pickle stores the user's access and refresh tokens, and is
    # created automatically when the authorization flow completes for the first
    # time.
    if(os.path.exists('token.pickle')):
        with open('token.pickle', 'rb') as token:
            creds = pickle.load(token)
    # If there are no (valid) credentials available, let the user log in.
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            flow = InstalledAppFlow.from_client_secrets_file('credentials.json', SCOPES)
            creds = flow.run_local_server(port=0)
        with open('token.pickle', 'wb') as token:
            pickle.dump(creds, token)

    service = build('calendar', 'v3', credentials=creds)



        # actual code
    #####################################################################################################################################################################

    #open json file with information
    file = sys.argv[1]
    with open(file, "r", encoding="utf-8") as jf:
        #for each events in the json file
        for e in json.load(jf):
            service.events().insert(calendarId='primary', body=e).execute()
    


def check_argv():
    
    if(len(sys.argv) == 1):
        print('Missing an argument.')
        return False

    if(len(sys.argv) > 2):
        print('Too many arguments.')
        return False

    if(not os.path.exists(sys.argv[1])):
        print('File does not exist.')
        return False
        
    if(not sys.argv[1].endswith('.json')):
        print('File must be a json file.')
        return False

    #if non returned false
    return True

        

if(__name__ == '__main__'):

    # check if arguments are valid then start script
    if(check_argv()):
        main()