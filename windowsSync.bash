#!/bin/bash

inputFile="daily.txt"
LOG_DIR="/mnt/user/logs/dailySync/"`date +"%m-%d-%Y"`""
TRASH_PATH="/mnt/user/trash/"
BACKUP_PATH="/mnt/disks/Backup"

LOG_FILEPATH="$LOG_DIR"/data-backup.txt""
HAD_ERROR=0

#Make log directory if not exists
mkdir -p "$LOG_DIR"

# INPUT FILE FORMAT: input path alone (no extra space, commas, etc.) or inputpath,outputpath if you need to change the output for specific input paths
file=$(cat $inputFile)
declare paths=()

# GET ARRAY OF PATHS FROM file
index=0
for line in $file
do
    filtered_line=$(echo -e "$line" | sed 's/\r//g')
    paths[$index]="$filtered_line"
    ((index+=1))
done

#--------------------------------------------------------------------------------------------
#--------------------------------------------------------------------------------------------

echo -e "Starting Backup..." >> "$LOG_FILEPATH"

for path in ${paths[@]}; do 

    pathArray=(`echo $path | tr ',' ' '`)

    echo -e "\n\n---------------------------------------------------------------------------------------------------------------------------------------------------" >> "$LOG_FILEPATH"
    echo -e "\n-------- BACKING UP: ${pathArray[0]} ------------------------------------------------------------------------------------------------------------------------\n" >> "$LOG_FILEPATH"
    echo -e "---------------------------------------------------------------------------------------------------------------------------------------------------\n\n" >> "$LOG_FILEPATH"

    if [ ${#pathArray[@]} == 1 ]; then
        SRC=${pathArray[0]}
        DEST=$BACKUP_PATH
    else
        SRC=${pathArray[0]}
        DEST=${pathArray[1]}
    fi

    # Sync files between two locations, move differing/deleted items to trash directory
    rsync -xavh --progress --delete --backup --backup-dir="$TRASH_PATH" "$SRC" "$DEST" &>> "$LOG_FILEPATH"

    # Send an error email if rsync exits with non 0 exit code.
    if [ $? -ne 0 ]; then
        HAD_ERROR=$?
        /usr/local/emhttp/webGui/scripts/notify -e "Daily Sync Status" -i alert -s "Daily Sync Error!!!" -d "Error: rsync exited with error code $? while backup up ${pathArray[0]}"
    fi

done